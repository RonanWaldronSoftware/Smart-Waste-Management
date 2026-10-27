using SmartWaste.Handler.AppSettings;
using SmartWaste.Handler.Controllers;
using SmartWaste.Model.Configuration;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.Resources;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using AutoMapper;

namespace SmartWaste.Handler.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContextFactory<RepositoryContext>(o => o.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddHttpClient();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<LocalizerService>();

            // Set up que subscribe controller
            QueueSubscribe.QueSubscriber = new UnitHandlerController(
                services.BuildServiceProvider().GetService<IServiceManager>(),
                services.BuildServiceProvider().GetService<ILogger<UnitHandlerController>>(),
                services.BuildServiceProvider().GetService<IDbContextFactory<RepositoryContext>>(),
                services.BuildServiceProvider().GetService<IMapper>());
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Handler service - SMART WASTE API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, Array.Empty<string>()}
                });
            });
        }

        /// <summary>
        /// Configure JWT and create scope
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration
            var jwtConfiguration = new JwtConfiguration();
            configuration.Bind(JwtConfiguration.Section, jwtConfiguration);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfiguration.ValidIssuer,
                    ValidAudience = jwtConfiguration.ValidAudience,
                    IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowedScope", policy =>
                {
                    policy.RequireClaim("scope", jwtConfiguration.RequiredScope);
                });
            });
        }


        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration) =>
                services.Configure<Configuration>(configuration.GetSection(Configuration.Section));
    }
}