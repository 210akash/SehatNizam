using Microsoft.Extensions.DependencyInjection;
using ERP.Mediator.Mediator.Auth.Validator;

namespace ERP.API.Helpers
{
    public static class ValidatorsHelper
    {
        public static void ConfigureValidators(this IServiceCollection services)
        {
            services.AddTransient<RegisterValidator>();
            services.AddTransient<UpdateValidator>();
            services.AddTransient<ForgetPasswordValidator>();
        }
    }
}
