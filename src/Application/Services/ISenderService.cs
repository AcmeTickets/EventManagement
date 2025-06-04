namespace EventManagement.Application.Services;

public interface ISenderService
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default);
    Task SendAsync<T>(T command, string destinationEndpoint, CancellationToken cancellationToken = default);
}
