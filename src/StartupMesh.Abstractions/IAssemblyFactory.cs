using System.Collections.Generic;
using System.Reflection;

namespace StartupMesh
{
    /// <summary>
    /// Discovery assemblies for startup process.
    /// </summary>
    public interface IAssemblyFactory
    {
        /// <summary>
        /// Gets assemblies to scan and discover startup types.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Assembly> GetAssemblies();
    }
}
