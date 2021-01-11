using System;

namespace StartupMesh
{
    /// <summary>
    /// Provides fluent apis for startup workflow configuration.
    /// </summary>
    public interface IStartupBuilder
    {
        /// <summary>
        /// Build new instance of <see cref="IStartupRunner"/> according to the startup workflow configuration.
        /// </summary>
        /// <returns></returns>
        IStartupRunner Build();

        /// <summary>
        /// Includes the specified startup type.
        /// </summary>
        /// <param name="type">The types to be included to the startup process.</param>
        /// <returns></returns>
        IStartupBuilder Include(Type type);

        /// <summary>
        /// Excludes the specified startup type.
        /// </summary>
        /// <param name="type">The types to be excluded from the startup process.</param>
        /// <returns></returns>
        IStartupBuilder Exclude(Type type);

        /// <summary>
        /// Specify the component to be used by the startup process.
        /// </summary>
        /// <param name="type">The requested type of the component.</param>
        /// <param name="component">The instance of the component.</param>
        /// <returns></returns>
        IStartupBuilder Use(Type type, object component);
    }

    public static class StartupBuilderExtensions
    {
        /// <summary>
        /// Includes the specified startup type.
        /// </summary>
        /// <typeparam name="T">The types to be included to the startup process.</typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IStartupBuilder Include<T>(this IStartupBuilder builder) where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Include(typeof(T));
        }

        /// <summary>
        /// Excludes the specified startup type.
        /// </summary>
        /// <typeparam name="T">The types to be excluded from the startup process.</typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IStartupBuilder Exclude<T>(this IStartupBuilder builder) where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Exclude(typeof(T));
        }

        /// <summary>
        /// Specify the component to be used by the startup process.
        /// </summary>
        /// <typeparam name="T">The requested type of the component.</typeparam>
        /// <param name="builder"></param>
        /// <param name="component">The instance of the component.</param>
        /// <returns></returns>
        public static IStartupBuilder Use<T>(this IStartupBuilder builder, T component)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Use(typeof(T), component);
        }
    }
}
