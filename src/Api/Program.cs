using NServiceBus;
using EventManagement.Application.Services;
using EventManagement.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using EventManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();



// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers(); // Add support for controllers
builder.Services.AddOpenApi();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();

// Register infrastructure and NServiceBus BEFORE builder.Build()

async Task ConfigureNServiceBusAsync(WebApplicationBuilder builder)
{
    var endpointConfig = builder.Environment.IsDevelopment()
        ? NServiceBusConfigurator.DevelopmentConfiguration(
            builder.Configuration,
            "EventManagement.Api",
            routingSettings =>
            {
                // Configure routing settings here if needed
                // Example: routingSettings.RouteToEndpoint(typeof(MyCommand), "MyDestinationEndpoint");
            })
        : NServiceBusConfigurator.ProductionConfiguration(
            builder.Configuration,
            "EventManagement.Api",
            routingSettings =>
            {
                // Configure routing settings here if needed
                // Example: routingSettings.RouteToEndpoint(typeof(MyCommand), "MyDestinationEndpoint");
            });

    var endpointWithContainer = EndpointWithExternallyManagedContainer.Create(endpointConfig, builder.Services);
    builder.Services.AddSingleton(p => endpointWithContainer.MessageSession.Value);
    await endpointWithContainer.Start(builder.Services.BuildServiceProvider());
}

await ConfigureNServiceBusAsync(builder);

DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration, builder.Environment.IsDevelopment());


var app = builder.Build();

// app.Lifetime.ApplicationStarted.Register(() =>
// {
//     var messageSession = app.Services.GetService<IMessageSession>();
//     if (messageSession != null)
//     {
//         var publisher = new NServiceBusEventPublisher(messageSession);
//         NServiceBusEventPublisherAccessor.Instance = publisher;
//     }
// });

app.MapOpenApi(); // Exposes the OpenAPI document at /openapi/v1.json

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

