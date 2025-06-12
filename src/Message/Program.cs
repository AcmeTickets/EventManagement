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

// NServiceBus endpoint configuration
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

// Register NServiceBus as a hosted service
builder.UseNServiceBus(endpointConfig);

// Register infrastructure and NServiceBus BEFORE builder.Build()
DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration, builder.Environment.IsDevelopment());

var host = builder.Build();

await host.RunAsync();