using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StartupMesh
{
    public class StartupRunner : IStartupRunner
    {
        private readonly ComponentCollection _components;
        private readonly StartupInvoker[] _invokers;

        internal StartupRunner(ComponentCollection components, IEnumerable<Type> includeTypes, IEnumerable<Type> excludeTypes)
        {
            _components = components;

            _invokers = DiscoverStartupTypes(includeTypes, excludeTypes)
                .Select(type => new StartupInvoker(type)).ToArray();

            foreach (var invoker in _invokers)
            {
                invoker.RunClassConstructor();
            }

            foreach (var invoker in _invokers)
            {
                invoker.RunInstanceConstructor(_components);
            }
        }

        private IEnumerable<Type> DiscoverStartupTypes(IEnumerable<Type> includeTypes, IEnumerable<Type> excludeTypes)
        {
            var filters = (IAssemblyFilter[])_components.GetAll(typeof(IAssemblyFilter));
            var providers = (IStartupProvider[])_components.GetAll(typeof(IStartupProvider));
            return ((IAssemblyFactory[])_components.GetAll(typeof(IAssemblyFactory)))
                .SelectMany(factory => factory.GetAssemblies())
                .Where(assembly =>
                {
                    foreach (var filter in filters)
                    {
                        var result = filter.Predicate(assembly);
                        if (result.HasValue) return result.Value;
                    }

                    return true;
                })
                .Aggregate(new List<Type>(), (types, assembly) =>
                {
                    types.AddRange(DiscoverStartupTypes(providers, assembly));
                    return types;
                }).Except(excludeTypes).Union(includeTypes);
        }

        private IEnumerable<Type> DiscoverStartupTypes(IStartupProvider[] providers, Assembly assembly)
        {
            var startupTypes = new HashSet<Type>();
            foreach (var provider in providers)
            {
                foreach (var type in provider.GetStartup(assembly))
                {
                    startupTypes.Add(type);
                }
            }

            return startupTypes;
        }

        public void Dispose()
        {
            Invoke("Dispose");
        }

        public void Invoke(string methodName)
        {
            foreach (var invoker in _invokers)
            {
                invoker.RunMethods(_components, methodName);
            }
        }

        public void Use<T>(T component)
        {
            _components.Add(typeof(T), component);
        }
    }
}
