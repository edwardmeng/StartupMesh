using System;
using Microsoft.Extensions.DependencyInjection;

namespace StartupMesh.Hosting
{
    public static class StartupMesherExtensions
    {
        public static void ConfigureServices(this IStartupRunner runner, IServiceCollection services)
        {
            if (runner == null)
            {
                throw new ArgumentNullException(nameof(runner));
            }
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            runner.Use(services);
            runner.Invoke("ConfigureServices");
        }

        public static void Configure(this IStartupRunner runner, IServiceProvider serviceProvider)
        {
            if (runner == null)
            {
                throw new ArgumentNullException(nameof(runner));
            }
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
   
            runner.Use(serviceProvider);
            runner.Invoke("Configure");
        }
    }
}
