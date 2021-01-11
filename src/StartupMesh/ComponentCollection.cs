using System;
using System.Collections;
using System.Collections.Generic;

namespace StartupMesh
{
    internal class ComponentCollection
    {
        private readonly Dictionary<Type, ArrayList> _components = new Dictionary<Type, ArrayList>();
        private readonly HashSet<IServiceProvider> _serviceProviders = new HashSet<IServiceProvider>();

        public void Add(Type type, object component)
        {
            if (!_components.TryGetValue(type, out var list))
            {
                list = new ArrayList();
                _components.Add(type, list);
            }

            list.Add(component);

            if (type == typeof(IServiceProvider))
            {
                _serviceProviders.Add((IServiceProvider)component);
            }
        }

        public object Get(Type type)
        {
            if (!_components.TryGetValue(type, out var list) || list.Count == 0)
            {
                foreach (var serviceProvider in _serviceProviders)
                {
                    var instance = serviceProvider.GetService(type);
                    if (instance != null)
                    {
                        return instance;
                    }
                }

                return null;
            }

            return list[list.Count - 1];
        }

        public Array GetAll(Type type)
        {
            var components = new ArrayList();

            foreach (var serviceProvider in _serviceProviders)
            {
                var instances = (ICollection) serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(type));
                if (instances != null)
                {
                    components.AddRange(instances);
                }
            }

            if (_components.TryGetValue(type, out var list))
            {
                components.AddRange(list);
            }

            return components.ToArray(type);
        }
    }
}
