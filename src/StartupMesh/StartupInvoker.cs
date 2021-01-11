using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StartupMesh
{
    internal class StartupInvoker
    {
        public StartupInvoker(Type clrType)
        {
            ClrType = clrType;
        }

        public Type ClrType { get; }

        public object Instance { get; set; }

        public void RunClassConstructor()
        {
            var constructor = ClrType.GetTypeInfo().DeclaredConstructors.FirstOrDefault(ctor => ctor.IsStatic);
            if (constructor != null)
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(ClrType.TypeHandle);
            }
        }

        public void RunInstanceConstructor(ComponentCollection components)
        {
            if (ClrType.IsAbstract || ClrType.IsGenericTypeDefinition || ClrType.IsInterface || ClrType.IsEnum) return;
            var constructors = ClrType.GetConstructors().Where(CheckMethod)
                .OrderByDescending(ctor => ctor.GetParameters().Length).ToArray();
            List<Type> missingTypes = new List<Type>();
            foreach (var constructor in constructors)
            {
                GetParameters(components, constructor, out var arguments, out var types);
                if (types == null || types.Length == 0)
                {
                    Instance = constructor.Invoke(arguments);
                    return;
                }

                missingTypes.AddRange(types);
            }

            throw new InvalidOperationException(
                $"The startup type cannot be instantiated: {TypeNameHelper.GetTypeDisplayName(ClrType)}" +
                Environment.NewLine +
                $"The parameter types cannot be resolved: {string.Join(", ", missingTypes.Select(type => TypeNameHelper.GetTypeDisplayName(type)))}");
        }

        public void RunMethods(ComponentCollection components, string name)
        {
            foreach (var method in ClrType.GetTypeInfo().GetDeclaredMethods(name)
                .Where(method => method.IsPublic)
                .Where(CheckMethod)
                .OrderByDescending(method => method.GetParameters().Length))
            {
                GetParameters(components, method, out var arguments, out var missingTypes);
                if (missingTypes == null || missingTypes.Length == 0)
                {
                    method.Invoke(method.IsStatic ? null : Instance, arguments);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"The method {name} in the activator type {TypeNameHelper.GetTypeDisplayName(ClrType)} cannot be invoked." +
                        Environment.NewLine +
                        $"The parameter types cannot be resolved: {string.Join(", ", missingTypes.Select(type => TypeNameHelper.GetTypeDisplayName(type)))}");
                }
            }
        }

        private bool CheckMethod(MethodBase method)
        {
            if (method.IsGenericMethodDefinition) return false;

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.IsOut || parameter.ParameterType.IsByRef) return false;
            }

            return true;
        }

        private void GetParameters(ComponentCollection components, MethodBase method, out object[] parameterValues, out Type[] missingTypes)
        {
            var parameters = method.GetParameters();
            var arguments = new object[parameters.Length];
            var types = new List<Type>();
            for (int i = 0; i < parameters.Length; i++)
            {
                var pi = parameters[i];
                if (TryGetParameterValue(components, pi, out var argumentValue))
                {
                    arguments[i] = argumentValue;
                }
                else
                {
                    types.Add(pi.ParameterType);
                }
            }

            parameterValues = arguments;
            missingTypes = types.ToArray();
        }

        private bool TryGetParameterValue(ComponentCollection components, ParameterInfo parameter, out object value)
        {
            var parameterType = parameter.ParameterType;
            bool isCollection = false;
            Type elementType = null;
            if (parameterType.IsArray)
            {
                isCollection = true;
                elementType = parameterType.GetElementType();
            }

            if (parameterType.IsGenericType)
            {
                var genericDefinition = parameterType.GetGenericTypeDefinition();
                if (genericDefinition == typeof(IEnumerable<>) || genericDefinition == typeof(ICollection<>) ||
                    genericDefinition == typeof(IReadOnlyCollection<>) || genericDefinition == typeof(IList<>) ||
                    genericDefinition == typeof(IReadOnlyList<>))
                {
                    isCollection = true;
                    elementType = parameterType.GenericTypeArguments[0];
                }
            }

            if (isCollection)
            {
                var array = components.GetAll(elementType);
                if (array.Length > 0)
                {
                    value = array;
                    return true;
                }
            }
            else
            {
                value = components.Get(parameterType);
                if (value != null) return true;
            }

            if (TryGetDefaultValue(parameter, out var valueProvider))
            {
                value = valueProvider();
                return true;
            }

            value = null;
            return false;
        }

        private bool TryGetDefaultValue(ParameterInfo pi, out Func<object> valueProvider)
        {
            bool hasDefaultValue;
            var tryToGetDefaultValue = true;
            try
            {
                // Workaround for https://github.com/dotnet/corefx/issues/17943
                if (pi.Member.DeclaringType?.GetTypeInfo().Assembly.IsDynamic ?? true)
                {
                    hasDefaultValue = pi.DefaultValue != null && pi.HasDefaultValue;
                }
                else
                {
                    hasDefaultValue = pi.HasDefaultValue;
                }
            }
            catch (FormatException) when (pi.ParameterType == typeof(DateTime))
            {
                // Workaround for https://github.com/dotnet/corefx/issues/12338
                // If HasDefaultValue throws FormatException for DateTime
                // we expect it to have default value
                hasDefaultValue = true;
                tryToGetDefaultValue = false;
            }

            if (hasDefaultValue)
            {
                valueProvider = () =>
                {
                    if (!tryToGetDefaultValue)
                    {
                        return default(DateTime);
                    }

                    var defaultValue = pi.DefaultValue;

                    // Workaround for https://github.com/dotnet/corefx/issues/11797
                    if (defaultValue == null && pi.ParameterType.GetTypeInfo().IsValueType)
                    {
                        defaultValue = Activator.CreateInstance(pi.ParameterType);
                    }

                    return defaultValue;
                };
                return true;
            }

            valueProvider = null;
            return false;
        }
    }
}
