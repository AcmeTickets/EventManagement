using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.Json;
using System.Reflection;
using NServiceBus;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Logging.AddConsole();

var endpointConfiguration = new EndpointConfiguration("EventManagement.Message");
endpointConfiguration.Conventions()
    .DefiningEventsAs(type => type.Namespace != null && type.Namespace.EndsWith("Events"))
    .DefiningCommandsAs(type => type.Namespace != null && type.Namespace.EndsWith("Commands"));

var connectionString = builder.Configuration.GetConnectionString("AzureServiceBus");
if (string.IsNullOrWhiteSpace(connectionString) || !connectionString.Contains("Endpoint=sb://") || !connectionString.Contains("SharedAccessKeyName=") || !connectionString.Contains("SharedAccessKey="))
{
    throw new InvalidOperationException("AzureServiceBus connection string is missing, empty, or does not appear to be a valid Azure Service Bus connection string.");
}
var transport = endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString, TopicTopology.Default));
endpointConfiguration.UseSerialization<SystemJsonSerializer>();

//endpointConfiguration.AuditProcessedMessagesTo("audit");

// Operational scripting: https://docs.particular.net/transports/azure-service-bus/operational-scripting
//endpointConfiguration.EnableInstallers();

builder.UseNServiceBus(endpointConfiguration);
var host = builder.Build();

await host.RunAsync();