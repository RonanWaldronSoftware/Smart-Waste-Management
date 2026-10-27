using SmartWaste.Unit.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogger();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.ConfigureJWT(builder.Configuration);

// Add services to the container.
builder.Services.RegisterServices();

// Add configuration
builder.Services.AddConfiguration(builder.Configuration);
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
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
