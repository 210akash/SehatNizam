//-----------------------------------------------------------------------
// <copyright file="CorsHelper.cs" company="sensyrtech">
//     transfercopy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API.Helpers
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The cross origin Helper
    /// </summary>
    public static class CorsHelper
    {
        /// <summary>
        /// Configures the cross origin service.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void ConfigureCorsService(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = configuration.GetSection("WebApplication:AllowOrigions").Get<string[]>();

            if (origins != null && origins.Length > 0)
            {
                services.AddCors(o =>
                    o.AddPolicy("CorsPolicy", builder =>
                    {
                        builder.WithOrigins(origins)
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    }));
            }
        }
    }
}
