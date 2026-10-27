using SmartWaste.Notification.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogger();
builder.Services.ConfigureSqlContext(builder.Configuration);

// Add configuration
builder.Services.AddConfiguration(builder.Configuration);

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

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
