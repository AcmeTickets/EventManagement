using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.Json;
using System.Reflection;
using NServiceBus;
using EventManagement.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections;

var builder = Host.CreateApplicationBuilder(args);

using var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
    logging.AddConsole();
});
var startupLogger = loggerFactory.CreateLogger("Startup");


startupLogger.LogInformation($"DOTNET_ENVIRONMENT = {Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}");

// Log the value of AzureServiceBus from each configuration provider
var configRoot = (IConfigurationRoot)builder.Configuration;
foreach (var provider in configRoot.Providers)
{
    if (provider.TryGet("ConnectionStrings:AzureServiceBus", out var value))
    {
        startupLogger.LogInformation($"Provider {provider} supplies ConnectionStrings:AzureServiceBus = '{value}'");
    }
    else
    {
        startupLogger.LogInformation($"Provider {provider} does NOT supply ConnectionStrings:AzureServiceBus");
    }
}

if (builder.Environment.IsDevelopment())
{

    var epc = NServiceBusConfigurator.DevelopmentConfiguration(
        builder.Configuration,
        "EventManagement.Api",
        routingSettings =>
        {
            // Configure routing settings here if needed
            // Example: routingSettings.RouteToEndpoint(typeof(MyCommand), "MyDestinationEndpoint");
        });
    var endpointWithExternallyManagedContainer = EndpointWithExternallyManagedContainer
        .Create(epc, builder.Services);

    builder.Services.AddSingleton(p => endpointWithExternallyManagedContainer.MessageSession.Value);
    var endpoint = await endpointWithExternallyManagedContainer.Start(builder.Services.BuildServiceProvider());
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
    var endpointWithExternallyManagedContainer = EndpointWithExternallyManagedContainer
        .Create(epc, builder.Services);

    builder.Services.AddSingleton(p => endpointWithExternallyManagedContainer.MessageSession.Value);
    var endpoint = await endpointWithExternallyManagedContainer.Start(builder.Services.BuildServiceProvider());
}

// Register infrastructure and NServiceBus BEFORE builder.Build()
DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration, builder.Environment.IsDevelopment());

var host = builder.Build();

await host.RunAsync();