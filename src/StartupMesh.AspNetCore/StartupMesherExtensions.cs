using System;
using Microsoft.AspNetCore.Builder;

namespace StartupMesh
{
    public static class StartupMesherExtensions
    {
        public static void Configure(this IStartupRunner runner,  IApplicationBuilder app)
        {
            if (runner == null)
            {
                throw new ArgumentNullException(nameof(runner));
            }
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            runner.Use(app.ApplicationServices);
            runner.Use(app);
            runner.Invoke("Configure");
        }
    }
}
