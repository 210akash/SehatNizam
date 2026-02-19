//-----------------------------------------------------------------------
// <copyright file="AuthenticationHelper.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API.Helpers
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Authentication Helper
    /// </summary>
    public static class AuthenticationHelper
    {
        /// <summary>
        /// Configures the JWT service.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="key">The key.</param>
        public static void ConfigureJWTService(this IServiceCollection services, string key)
        {
            services.AddAuthentication(x =>
             {
                 x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             })
             .AddJwtBearer(x =>
             {
                 var keys = Encoding.ASCII.GetBytes(key);
                 x.RequireHttpsMetadata = false;
                 x.SaveToken = true;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(keys),
                     LifetimeValidator = TokenLifetimeValidator.Validate,
                 };
                 x.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var accessToken = context.Request.Query["access_token"].ToString();
                         var path = context.HttpContext.Request.Path;
                         if (string.IsNullOrEmpty(accessToken) == false &&
                         (path.StartsWithSegments("/NotificationHub") || path.StartsWithSegments("/CommentHub") || path.StartsWithSegments("/AssetNotificationHub")))
                         {
                             if (!string.IsNullOrEmpty(accessToken) && accessToken.Split(' ').Length > 1)
                             {

                                 context.Token = accessToken.Split(' ')[1];
                             }
                         }
                         if (path.StartsWithSegments("/api/Asset/NFCCardReader"))
                         {
                             var Token = context.Request.ReadFormAsync().Result.FirstOrDefault(s => s.Key == "authorization").Value.ToString();
                             if (!string.IsNullOrEmpty(Token) && Token.Split(' ').Length > 1)
                             {
                                 context.Token = Token.Split(' ')[1];
                             }
                         }

                         return Task.CompletedTask;
                     }
                 };
             });
        }
    }
}
