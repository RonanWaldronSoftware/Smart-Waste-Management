using SmartWaste.Model.Core;
using SmartWaste.Model.OldCore;
using SmartWaste.Model.Resources;
using SmartWaste.OldRepository;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.EntityFrameworkCore;
using SmartWaste.Model.Configuration;

namespace SmartWaste.CronJob.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlServerContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<MediatoriumSmartWasteContext>(options =>
                options.UseMySQL(configuration.GetConnectionString("OldDefaultConnection")));

        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration) =>
            services.Configure<Configuration>(configuration.GetSection(Configuration.Section));

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpClient();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IOldRepositoryManager, OldRepositoryManager>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<LocalizerService>();
            services.AddSingleton<SingletonNotReporting>();
        }
    }
}
