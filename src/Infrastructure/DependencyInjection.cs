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
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Cosmos DB setup
        var cosmosConnectionString = configuration["CosmosDb:ConnectionString"];
        var databaseName = configuration["CosmosDb:DatabaseName"];
        var containerName = configuration["CosmosDb:ContainerName"];

        services.AddSingleton(sp =>
        {
            var client = new CosmosClient(cosmosConnectionString);
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
        services.AddSingleton<ISenderService, NServiceBusEventPublisher>();

        return services;
    }
}
