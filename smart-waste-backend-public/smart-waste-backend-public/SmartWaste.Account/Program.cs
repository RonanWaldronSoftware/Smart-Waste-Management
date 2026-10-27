using SmartWaste.Account.Extensions;
using SmartWaste.Model.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.AddAuthorization();

builder.ConfigureLogger();

// Configuration
builder.Services.Configure<Configuration>(builder.Configuration.GetSection(Configuration.Section));

// Add services to the container.
builder.Services.RegisterServices();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization("AllowedScope");

app.MapHealthChecks("/health");

app.Run();
