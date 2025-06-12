using EventManagement.Domain.Entities;
using EventManagement.Domain.Repositories;
using Microsoft.Azure.Cosmos;

namespace EventManagement.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly Container _container;

    public EventRepository(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        _container = cosmosClient.GetContainer(databaseName, containerName);
    }

    public async Task<Event> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
            var response = await _container.ReadItemAsync<Event>(
                id.ToString(),
                new PartitionKey(id.ToString()),
                cancellationToken: cancellationToken);
            return response.Resource;
     
    }

    public async Task AddAsync(Event evt, CancellationToken cancellationToken)
    {
    // Log the evt payload as JSON for debugging
    var json = System.Text.Json.JsonSerializer.Serialize(evt);
    Console.WriteLine($"[CosmosDB] Payload: {json}");
        await _container.CreateItemAsync(evt, new PartitionKey(evt.Id.ToString().ToLower()), cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Event evt, CancellationToken cancellationToken)
    {
        await _container.ReplaceItemAsync(evt, evt.Id.ToString(), new PartitionKey(evt.Id.ToString()), cancellationToken: cancellationToken);
    }
}