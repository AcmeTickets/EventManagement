using EventManagement.Domain.Repositories;
using EventManagement.Domain.Services;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Infrastructure.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EventManagement.Application.Services;

namespace EventManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        // Cosmos DB setup
        var databaseName = configuration["CosmosDb:DatabaseName"] ?? throw new InvalidOperationException("CosmosDb:DatabaseName is missing in configuration.");
        var containerName = configuration["CosmosDb:ContainerName"] ?? throw new InvalidOperationException("CosmosDb:ContainerName is missing in configuration.");

        services.AddSingleton(sp =>
        {
            CosmosClient client;
            if (isDevelopment)
            {
                var cosmosConnectionString = configuration["CosmosDb:ConnectionString"];
                client = new CosmosClient(cosmosConnectionString);
            }
            else
            {
                var cosmosEndpoint = configuration["CosmosDb:AccountEndpoint"];
                var credential = new Azure.Identity.DefaultAzureCredential();
                client = new CosmosClient(cosmosEndpoint, credential);
            }
            return client;
        });

        services.AddSingleton<IEventRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            return new EventRepository(client, databaseName, containerName);
        });

        // External HTTP service
        services.AddHttpClient<IExternalService, ExternalService>(client =>
        {
            client.BaseAddress = new Uri("https://api.example.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register NServiceBus event publisher
        // services.AddSingleton<ISenderService>(sp =>
        // {
        //     var messageSession = sp.GetRequiredService<IMessageSession>();
        //     return new NServiceBusEventPublisher(messageSession);
        //     return senderService;
        // });
        
       // services.AddSingleton<ISenderService, NServiceBusEventPublisher>();

        // Register IEventService using EventService and resolve ISenderService from the accessor if not provided by DI
 


        return services;
    }
}
