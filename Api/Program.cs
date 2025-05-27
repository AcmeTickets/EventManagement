using NServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();

var endpointConfiguration = new EndpointConfiguration("EventManagement.Api");
endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.Conventions()
    .DefiningEventsAs(type => type.Namespace != null && type.Namespace.EndsWith("Events"))
    .DefiningCommandsAs(type => type.Namespace != null && type.Namespace.EndsWith("Commands"));
    

// Debug: Print all environment variables related to connection strings
foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
{
    if (envVar is string key && key.Contains("AzureServiceBus", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"[DEBUG] Env: {key} = '{Environment.GetEnvironmentVariable(key)}'");
    }
}

// Debug: Print all configuration providers and their sources if possible
if (builder.Configuration is IConfigurationRoot configRoot)
{
    Console.WriteLine("[DEBUG] Configuration Providers (detailed):");
    foreach (var provider in configRoot.Providers)
    {
        Console.Write($"  - {provider.GetType().Name}");
        // Try to print the path for JsonConfigurationProvider
        if (provider is Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider jsonProvider)
        {
            var field = typeof(Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider).GetField("_source", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var source = field?.GetValue(jsonProvider);
            var pathProp = source?.GetType().GetProperty("Path");
            var path = pathProp?.GetValue(source) as string;
            if (!string.IsNullOrEmpty(path))
            {
                Console.Write($" (file: {path})");
            }
        }
        Console.WriteLine();
    }
}

// Debug: Print all key-value pairs in configuration for ConnectionStrings
Console.WriteLine("[DEBUG] All ConnectionStrings in configuration:");
foreach (var kvp in builder.Configuration.GetSection("ConnectionStrings").GetChildren())
{
    Console.WriteLine($"  - {kvp.Key}: '{kvp.Value}'");
}

// Debug: Print all top-level configuration keys
Console.WriteLine("[DEBUG] All top-level configuration keys:");
foreach (var section in builder.Configuration.GetChildren())
{
    Console.WriteLine($"  - {section.Key}");
}

// Debug: Print value from configuration
var configValue = builder.Configuration.GetConnectionString("AzureServiceBus");
Console.WriteLine($"[DEBUG] Configuration.GetConnectionString: '{configValue}'");





var connectionString = builder.Configuration.GetConnectionString("AzureServiceBus")
    ?? throw new InvalidOperationException("AzureServiceBus connection string is missing in configuration.");
var routing = endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString, TopicTopology.Default));


builder.UseNServiceBus(endpointConfiguration);
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

    //http://localhost:5271/swagger/index.html
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
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
