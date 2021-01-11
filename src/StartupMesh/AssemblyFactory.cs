using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StartupMesh
{
    internal class AssemblyFactory : IAssemblyFactory
    {
        private readonly string _folder;
        private readonly bool _recursive;

        public AssemblyFactory(string folder, bool recursive)
        {
            _folder = ResolvePath(folder);
            _recursive = recursive;
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            var option = _recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var assemblyPath in Directory.EnumerateFiles(_folder, "*.dll", option)
                .Concat(Directory.EnumerateFiles(_folder, "*.exe", option)))
            {
                var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(assembly => !assembly.IsDynamic && string.Equals(GetAssemblyPath(assembly), assemblyPath, StringComparison.OrdinalIgnoreCase)) ??
                    LoadAssemblyFile(assemblyPath);
                if (loadedAssembly != null) yield return loadedAssembly;
            }
        }

        private static Assembly LoadAssemblyFile(string assemblyPath)
        {
            try
            {
                return Assembly.LoadFile(assemblyPath);
            }
            // Not a managed dll/exe
            catch (Exception ex) when (ex is BadImageFormatException || ex is FileLoadException)
            {
            }

            return null;
        }

        private static string ResolvePath(string folder)
        {
            if (string.IsNullOrEmpty(folder) || !Path.IsPathRooted(folder))
            {
                folder = "~/" + folder;
            }

            if (folder.StartsWith("~/"))
            {
                var domainRootPath = Directory.GetCurrentDirectory();
                folder = Path.Combine(domainRootPath, folder.Substring(2));
            }

            return folder;
        }

        private static string GetAssemblyPath(Assembly assembly)
        {
            return assembly.IsDynamic ? assembly.FullName : new Uri(assembly.Location).LocalPath;
        }
    }
}
