using EventManagement.Application.Services;
using NServiceBus;

namespace EventManagement.Infrastructure.Services;

public class NServiceBusEventPublisher : ISenderService
{
    private readonly IMessageSession _messageSession;

    public NServiceBusEventPublisher(IMessageSession messageSession)
    {
        _messageSession = messageSession;
    }

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
    {
        // NServiceBus does not use the cancellation token directly
        return _messageSession.Publish(@event);
    }

    public Task SendAsync<T>(T command, string destinationEndpoint, CancellationToken cancellationToken = default)
    {
        // NServiceBus does not use the cancellation token directly
        return _messageSession.Send(destinationEndpoint, command);
    }
}
