using SmartWaste.Model.Configuration;
using SmartWaste.Model.Core;
using SmartWaste.Model.Resources;
using SmartWaste.Notification.AppSettings;
using SmartWaste.Notification.Controllers;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace SmartWaste.Notification.Extensions
{
    public static class ServiceExtensions
    {

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
           services.AddDbContextFactory<RepositoryContext>(o =>
           o.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpClient();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<LocalizerService>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IServiceManager, ServiceManager>();

            // Set up que subscribe controller
            QueueSubscribe.QueSubscriber = new QueueSubscriberController(
                services.BuildServiceProvider().GetService<ILogger<QueueSubscriberController>>(),
                services.BuildServiceProvider().GetService<IHttpContextAccessor>(),
                services.BuildServiceProvider().GetService<IServiceManager>(),
                services.BuildServiceProvider().GetService<IWebHostEnvironment>(),
                services.BuildServiceProvider().GetService<IOptions<Configuration>>()
            );
        }

        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration) =>
            services.Configure<Configuration>(configuration.GetSection(Configuration.Section));
    }
}
