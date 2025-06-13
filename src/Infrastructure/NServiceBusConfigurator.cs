using System;
using Azure.Messaging.ServiceBus;
using NServiceBus;
using NServiceBus.Transport.AzureServiceBus;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace EventManagement.Infrastructure
{
    public static class NServiceBusConfigurator
    {
        public static EndpointConfiguration ProductionConfiguration(IConfiguration configuration, string endpointName, Action<RoutingSettings<AzureServiceBusTransport>> routingSettingsConfiguration)
        {
            var endpointConfiguration = CreateEndpointConfiguration(endpointName);
            var fullyQualifiedNamespace = configuration["AzureServiceBus:FullyQualifiedNamespace"];
            if (string.IsNullOrWhiteSpace(fullyQualifiedNamespace))
                throw new InvalidOperationException("AzureServiceBus:FullyQualifiedNamespace is missing in configuration.");
            var transportWithTokenCredentials = new AzureServiceBusTransport(fullyQualifiedNamespace,new DefaultAzureCredential(), TopicTopology.Default);
            var transportExtensions = endpointConfiguration.UseTransport(transportWithTokenCredentials);
            //routingSettingsConfiguration(transportExtensions.Routing());
            return endpointConfiguration;
        }

        public static EndpointConfiguration DevelopmentConfiguration(IConfiguration configuration, string endpointName, Action<RoutingSettings<AzureServiceBusTransport>> routingSettingsConfiguration)
        {
            // Debug: Log the connection string value and all config sources
            var connectionString = configuration.GetConnectionString("AzureServiceBus");
            Console.WriteLine($"[NServiceBusConfigurator] AzureServiceBus connection string: '{connectionString}'");
     
            if (connectionString == null)
                throw new InvalidOperationException("AzureServiceBus connection string is missing in configuration.");
            var endpointConfiguration = CreateEndpointConfiguration(endpointName);
            var transport = new AzureServiceBusTransport(connectionString, TopicTopology.Default);
            var transportExtensions = endpointConfiguration.UseTransport(transport);
         //   routingSettingsConfiguration(transportExtensions.Routing());
            return endpointConfiguration;
        }

        private static EndpointConfiguration CreateEndpointConfiguration(string endpointName)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();
            endpointConfiguration.Conventions()
                .DefiningEventsAs(type => type.Namespace != null && type.Namespace.EndsWith("Events"))
                .DefiningCommandsAs(type => type.Namespace != null && type.Namespace.EndsWith("Commands"));
            endpointConfiguration.SetDiagnosticsPath("/tmp");
            var licensePath = Path.Combine(AppContext.BaseDirectory, "License.xml");
            endpointConfiguration.LicensePath(licensePath);
            return endpointConfiguration;
        }
    }
}