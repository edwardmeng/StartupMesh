using System;
using System.Collections.Generic;

namespace StartupMesh
{
    public class StartupBuilder : IStartupBuilder
    {
        private readonly HashSet<Type> _includeTypes = new HashSet<Type>();
        private readonly HashSet<Type> _excludeTypes = new HashSet<Type>();
        private readonly ComponentCollection _components = new ComponentCollection();

        public static IStartupBuilder Create() => new StartupBuilder();

        private StartupBuilder()
        {
            this.Use<IAssemblyFilter>(new AssemblyFilter())
                .Use<IStartupProvider>(new ConventionStartupProvider())
                .AddAssemblies(AppContext.BaseDirectory);
        }

        public IStartupRunner Build()
        {
            return new StartupRunner(_components, _includeTypes, _excludeTypes);
        }

        public IStartupBuilder Include(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _includeTypes.Add(type);
            _excludeTypes.Remove(type);
            return this;
        }

        public IStartupBuilder Exclude(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _excludeTypes.Add(type);
            _includeTypes.Remove(type);
            return this;
        }

        public IStartupBuilder Use(Type type, object component)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            _components.Add(type, component);
            return this;
        }
    }

    public static class StartupBuilderExtensions
    {
        public static IStartupBuilder AddAssemblies(this IStartupBuilder builder, string folder, bool recursive = true)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Use<IAssemblyFactory>(new AssemblyFactory(folder, recursive));
        }
    }
}
