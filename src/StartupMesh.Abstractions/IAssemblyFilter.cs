using System.Reflection;

namespace StartupMesh
{
    /// <summary>
    /// Determines assemblies to scan for types
    /// </summary>
    public interface IAssemblyFilter
    {
        /// <summary>
        /// Determines if assembly is eligible to be scanned, return true to allow scanning, otherwise false.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        bool? Predicate(Assembly assembly);
    }
}
