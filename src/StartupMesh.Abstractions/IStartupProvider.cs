using System;
using System.Collections.Generic;
using System.Reflection;

namespace StartupMesh
{
    public interface IStartupProvider
    {
        IEnumerable<Type> GetStartup(Assembly assembly);
    }
}
