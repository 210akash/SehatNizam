//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using ERP.API.Services;
    using Serilog;
    using Serilog.Enrichers;
    using System;
    using Microsoft.Extensions.Configuration;
    using Serilog.Filters;

    /// <summary>
    /// The Program Class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>

        [Obsolete]
        public static void Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration)
            //    //.MinimumLevel.Information()
            //    .Filter.ByIncludingOnly(evt => evt.Properties.ContainsKey("Leads"))
            //    .WriteTo.MSSqlServer(
            //        connectionString: "Data Source=192.168.0.37;Initial Catalog=CRM_RealState_Test;Persist Security Info=True;User ID=webUser;Password=Master@123;MultipleActiveResultSets=true;TrustServerCertificate=True;",
            //        tableName: "Logger", // Table name to store logs
            //        autoCreateSqlTable: true // Create table if it doesn't exist
            //    )
            //    .CreateLogger();

            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                ServiceConfigs.SeedDatabase(scope);
            }
            host.Run();
        }
        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The Host Builder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                });
    }
}
