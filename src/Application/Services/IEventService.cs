using EventManagement.Application.Commands;
using EventManagement.Application.DTOs;

namespace EventManagement.Application.Services;

public interface IEventService
{
    Task<EventDto> AddEventAsync(AddEventCommand command, CancellationToken cancellationToken);
    Task ExpireEventAsync(ExpireEventCommand command, CancellationToken cancellationToken);
    Task CloseEventAsync(CloseEventCommand command, CancellationToken cancellationToken);
}
