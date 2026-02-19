using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using ERP.BusinessModels.Enums;
using Microsoft.Data.SqlClient;
using System.IO;

namespace ERP.API.Services
{
    public class ServiceConfigs
    {
        public static void SeedDatabase(IServiceScope scope)
        {
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var hostEnvironment = services.GetRequiredService<IHostEnvironment>();
            string sqlConnectionString = configuration.GetSection("ConnectionStrings:DefaultConnectionString").Value;
            //FileInfo file = new FileInfo(Path.Combine(hostEnvironment.ContentRootPath, Constants.SQLScriptDefaultPath));
            //string script = file.OpenText().ReadToEnd();
            //SqlConnection conn = new SqlConnection(sqlConnectionString);
            //Server server = new Server(new ServerConnection(conn));
            //server.ConnectionContext.ExecuteNonQuery(script);
        }
    }
}
