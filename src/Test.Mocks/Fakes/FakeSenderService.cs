using EventManagement.Application.Services;

namespace EventManagement.Test.Mocks.Fakes;

public class FakeSenderService : ISenderService
{
    public List<object> PublishedEvents { get; } = new();
    public List<(object Command, string Destination)> SentCommands { get; } = new();

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
    {
        PublishedEvents.Add(@event!);
        return Task.CompletedTask;
    }

    public Task SendAsync<T>(T command, string destinationEndpoint, CancellationToken cancellationToken = default)
    {
        SentCommands.Add((command!, destinationEndpoint));
        return Task.CompletedTask;
    }
}
