using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.Json;
using System.Reflection;
using NServiceBus;
using EventManagement.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Logging.AddConsole();

//endpointConfiguration.AuditProcessedMessagesTo("audit");

// Operational scripting: https://docs.particular.net/transports/azure-service-bus/operational-scripting
//endpointConfiguration.EnableInstallers();

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
    builder.UseNServiceBus(epc);
    //http://localhost:5271/swagger/index.html

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

var host = builder.Build();

await host.RunAsync();