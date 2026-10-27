using SmartWaste.Model.Configuration;
using SmartWaste.Model.Core;
using SmartWaste.Model.Resources;
using SmartWaste.Repository;
using SmartWaste.Service;
using SmartWaste.Service.Contracts;
using SmartWaste.Service.Implementations;
using SmartWaste.Tcp.Services;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Tcp.Extensions
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
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<LocalizerService>();
            services.AddHostedService<TcpListenerService>();
        }
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration) =>
            services.Configure<Configuration>(configuration.GetSection(Configuration.Section));

    }
}
