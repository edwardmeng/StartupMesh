using System.Reflection;

namespace StartupMesh
{
    internal class AssemblyFilter : IAssemblyFilter
    {
        public bool? Predicate(Assembly assembly)
        {
            if (assembly.IsDynamic) return false;

            var name = assembly.GetName().Name.Split('.');

            switch (name[0].ToLower())
            {
                case "mscorlib":
                case "system":
                case "microsoft":
                    return false;
            }

            return null;
        }
    }
}
