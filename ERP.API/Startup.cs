namespace ERP.API
{
    using System;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using ERP.API.Helpers;
    using ERP.Services.Hubs;
    using ERP.API.Middleware;
    using ERP.BusinessModels.BaseVM;
    using ERP.Mediator.AutoMapper.Configuration;
    using ERP.Core.Identity;
    using ERP.Core.Provider;
    using ERP.Repositories.UnitOfWork;
    using SendGrid;
    using Microsoft.AspNetCore.Http;
    using ERP.Services.Settings;
    using Stripe;
    using Microsoft.AspNetCore.Http.Features;
    using Serilog;
    using ERP.Entities.Models;
    using Newtonsoft.Json;
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<ERPDbContext>(options =>
                     options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnectionString"), options1 =>
                     {
                         options1.CommandTimeout(300); // 3 minutes
                     }));
            //services.AddDbContext<SensyrtechContextCommon>(options =>
            //        options.UseSqlServer(this.Configuration.GetConnectionString("CommonConnectionString")));
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            //services.AddTransient<TokenManagerMiddleware>();
            //services.AddTransient<ITokenManager, TokenManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddDistributedRedisCache(r =>
            //{
            //    r.Configuration = Configuration["redis:connectionString"];
            //});
            // Configuration Helpers

            services.AddControllers()
.AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.ConfigureSwaggerService();

            // ERP business services
            services.ConfigureAepisleServices();

            services.ConfigureCorsService(this.Configuration);
            services.AddSignalR();
            services.AddIdentity<AspNetUsersModel, AspNetRolesModel>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            }).AddDefaultTokenProviders();

            services.ConfigureJWTService(
            this.Configuration["JwtSecurityToken:Key"]);

            // Settings
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUnitOfWorkDapper, UnitOfWorkDapper>(provider => new UnitOfWorkDapper(this.Configuration.GetConnectionString("DefaultConnectionString")));
            //services.AddTransient<IUnitOfWorkDapper, UnitOfWorkCommon>(provider => new UnitOfWorkCommon(this.Configuration.GetConnectionString("CommonConnectionString")));
            //services.AddTransient<IUnitOfWorkCommon, UnitOfWorkCommon>();

            services.Configure<EmailSetting>(this.Configuration.GetSection("EmailConfiguration"));
            services.Configure<TwilioSettings>(this.Configuration.GetSection("TwilioConfiguration"));

            // registering the identity services
            services.AddTransient<IUserStore<AspNetUsersModel>, CustomUserStore>();
            services.AddTransient<IRoleStore<AspNetRolesModel>, CustomRoleStore>();
            services.AddTransient<SendGridClient>((serviceProvider) => { return new SendGridClient(Configuration.GetSection("EmailConfiguration:SendGridApiKey").Value); });
            services.AddScoped<SessionProvider>();
            services.AddMemoryCache();
            // services.AddTransient<SensyrHistorySqlDepedency>();

            // Validators
            services.ConfigureValidators();

            // Identity Services
            services.AddAutoMapper(c => c.AddProfile<AutoMapperConfiguration>(), typeof(Startup));

            // add mediator
            var assembly = AppDomain.CurrentDomain.Load("ERP.Mediator");
            services.AddMediatR(assembly);

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue; // <-- ! long.MaxValue
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.MultipartHeadersCountLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            ////services.AddMvc().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserPreferencesModelValidator>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseCors("CorsPolicy");
            app.UseErrorHandlingMiddleware();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            //app.UseMiddleware<TokenManagerMiddleware>();
            app.UseMiddleware<SessionMiddleware>();

            // Enable middleware to serve generated Swagger as a JSON endpoint.

            //if (!env.IsProduction())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c =>
            //    {
            //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
            //        c.RoutePrefix = string.Empty;
            //    });
            //}

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/NotificationHub");
                endpoints.MapHub<AssetNotificationHub>("/AssetNotificationHub");
                endpoints.MapHub<CommentHub>("/CommentHub");
            });

        }
    }
}
