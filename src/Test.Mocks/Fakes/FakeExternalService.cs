using EventManagement.Domain.Services;

namespace EventManagement.Test.Mocks.Fakes;

public class FakeExternalService : IExternalService
{
    public List<(Guid EventId, string Status)> Notifications { get; } = new();

    public Task NotifyEventStatusAsync(Guid eventId, string status, CancellationToken cancellationToken)
    {
        Notifications.Add((eventId, status));
        return Task.CompletedTask;
    }
}
