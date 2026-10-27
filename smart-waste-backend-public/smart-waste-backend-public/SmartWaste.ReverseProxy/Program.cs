using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configuration for content cache policy
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("unitCachePolicy", builder => builder.Expire(TimeSpan.FromSeconds(60)));
});

// Configuration for rate limit for reduce DDOS attacks
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("rateLimiterPolicy", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromSeconds(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 1000;
    });
});

// Add configuration for reverse proxy
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseOutputCache();
app.UseRateLimiter();
app.MapReverseProxy();
app.MapHealthChecks("/health");
app.Run();
