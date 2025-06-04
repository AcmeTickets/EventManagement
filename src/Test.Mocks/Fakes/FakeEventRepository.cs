using EventManagement.Domain.Entities;
using EventManagement.Domain.Repositories;

namespace EventManagement.Test.Mocks.Fakes;

public class FakeEventRepository : IEventRepository
{
    private readonly Dictionary<Guid, Event> _events = new();

    public Task<Event> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _events.TryGetValue(id, out var evt);
        return Task.FromResult(evt);
    }

    public Task AddAsync(Event evt, CancellationToken cancellationToken)
    {
        _events[evt.Id] = evt;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Event evt, CancellationToken cancellationToken)
    {
        _events[evt.Id] = evt;
        return Task.CompletedTask;
    }
}
