using NServiceBus;
using EventManagement.Application.Services;
using EventManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IEventService, EventService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers(); // Add support for controllers
builder.Services.AddOpenApi();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();
    app.MapOpenApi(); // Exposes the OpenAPI document at /openapi/v1.json
if (app.Environment.IsDevelopment())
{
    var epc = NServiceBusConfigurator.DevelopmentConfiguration(
    builder.Configuration,
    "EventManagement.Api",
    routingSettings =>
    {
        // Configure routing settings here if needed
        // Example: routingSettings.RouteToEndpoint(typeof(MyCommand), "MyDestinationEndpoint");
    });
    builder.UseNServiceBus(epc);
    //http://localhost:5271/swagger/index.html
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}
else
{
    var epc = NServiceBusConfigurator.ProductionConfiguration(
    builder.Configuration,
    "EventManagement.Api",
    routingSettings =>
    {
        // Configure routing settings here if needed
        // Example: routingSettings.RouteToEndpoint(typeof(MyCommand), "MyDestinationEndpoint");
    });
    builder.UseNServiceBus(epc);
}

    app.UseHttpsRedirection();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
