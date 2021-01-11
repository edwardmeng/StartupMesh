using System;
using System.Collections.Generic;
using System.Reflection;

namespace StartupMesh
{
    internal class ConventionStartupProvider : IStartupProvider
    {
        public IEnumerable<Type> GetStartup(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;
            var typeNames = new[]
            {
                "Startup",
                assemblyName + ".Startup"
            };

            foreach (var typeName in typeNames)
            {
                var startupType = assembly.GetType(typeName, false, true);
                if (startupType != null)
                {
                    yield return startupType;
                }
            }
        }
    }
}
