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
            var endpointConfiguration = CreateEndpointConfiguration(endpointName);
            var connectionString = configuration.GetConnectionString("AzureServiceBus")
                ?? throw new InvalidOperationException("AzureServiceBus connection string is missing in configuration.");
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
            return endpointConfiguration;
        }
    }
}