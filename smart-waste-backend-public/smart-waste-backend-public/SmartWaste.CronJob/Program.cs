using SmartWaste.CronJob.Extensions;
using SmartWaste.Model.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure database contexts
builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.ConfigureSqlServerContext(builder.Configuration);

// Configure logging
builder.ConfigureLogger();

// Add configuration
builder.Services.AddConfiguration(builder.Configuration);

// Register other services
builder.Services.RegisterServices();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();