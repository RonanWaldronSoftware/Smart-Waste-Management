using Gelf.Extensions.Logging;

namespace SmartWaste.Account.Extensions
{
    public static class BuilderExtensions
    {
        public static void ConfigureLogger(this WebApplicationBuilder builder)
        {
            builder.Host.ConfigureLogging((context, builder) =>
            {
                // Read GelfLoggerOptions from appsettings.json.
                builder.Services.Configure<GelfLoggerOptions>(context.Configuration.GetSection("Graylog"));

                // Optionally configure GelfLoggerOptions further.
                builder.Services.PostConfigure<GelfLoggerOptions>(options =>
                    options.AdditionalFields["machine_name"] = Environment.MachineName);

                // Check if is development
                if (context.HostingEnvironment.IsDevelopment())
                {
                    builder.Services.PostConfigure<GelfLoggerOptions>(options =>
                        options.AdditionalFields["mode"] = "Development");
                }
                else
                {
                    builder.Services.PostConfigure<GelfLoggerOptions>(options =>
                        options.AdditionalFields["mode"] = "Production");
                }

                // Read Logging settings from appsettings.json and add providers.
                builder.AddConfiguration(context.Configuration.GetSection("Logging")).AddGelf();
            });
        }
    }
}
