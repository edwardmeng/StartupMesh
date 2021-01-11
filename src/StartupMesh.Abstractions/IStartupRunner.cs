using System;

namespace StartupMesh
{
    /// <summary>
    /// Represents the mesher to manage the application startup process.
    /// </summary>
    public interface IStartupRunner : IDisposable
    {
        /// <summary>
        /// Invoke the specified method in all the registered startup types. 
        /// </summary>
        /// <param name="methodName">The name of method to be invoked.</param>
        void Invoke(string methodName);

        /// <summary>
        /// Specify the component to be used by the startup process.
        /// </summary>
        /// <typeparam name="T">The requested type of the component.</typeparam>
        /// <param name="component">The instance of the component.</param>
        void Use<T>(T component);
    }
}
