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

        //services.AddCors(options =>
        //{
        //    options.AddPolicy("CorsPolicy",
        //        builder => builder
        //            .AllowAnyOrigin()
        //            .AllowAnyMethod()
        //            .AllowAnyHeader());
        //});

        //services.AddCors(o =>
        //       o.AddPolicy(
        //           "CorsPolicy",
        //           builder =>
        //                   {
        //                       builder.WithOrigins(configuration["WebApplication:AllowOrigions"].Split(';'))
        //                           .AllowAnyMethod()
        //                           .AllowAnyHeader()
        //                           .AllowCredentials();
        //                   }));
     
            services.AddCors(o =>
              o.AddPolicy(
                  "CorsPolicy",
                  builder =>
                  {
                      builder.WithOrigins("http://110.39.5.82:3112",
                                          "http://202.166.160.200:9081",
                                          "http://192.168.0.38:9081",
                                          "http://localhost:4200")
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                  }));
        }
    }
}
