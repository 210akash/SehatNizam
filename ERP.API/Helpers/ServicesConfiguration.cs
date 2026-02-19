//-----------------------------------------------------------------------
// <copyright file="ServicesConfiguration.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API.Helpers
{
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using ERP.Services.Interfaces;
    using NetCore.AutoRegisterDi;
    /// <summary>
    /// Authentication Helper
    /// </summary>
    public static class ServicesConfiguration
    {
        public static void ConfigureAepisleServices(this IServiceCollection services)
        {
            var assembliesToScan = Assembly.GetAssembly(typeof(IBlobService));
            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
              .Where(c => c.Name.EndsWith("Service"))
              .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
        }
    }
}
