using SmartWaste.Handler.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure sql
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.ConfigureLogger();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureJWT(builder.Configuration);

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

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
