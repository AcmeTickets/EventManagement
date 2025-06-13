using EventManagement.Domain.Events;
using Newtonsoft.Json;

namespace EventManagement.Domain.Entities;

public class Event
{
    [JsonProperty("id")]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required EventStatus Status { get; set; }
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Event() { } // For EF/ORM

    public static Event Create(string name, DateTime startDate, DateTime endDate)
    {
        var evt = new Event
        {
            Id = Guid.NewGuid(),
            Name = name,
            StartDate = startDate,
            EndDate = endDate,
            Status = EventStatus.Active
        };
        evt.AddDomainEvent(new EventCreatedEvent(evt.Id, evt.Name));
        return evt;
    }

    public void Expire()
    {
        if (Status != EventStatus.Active)
            throw new InvalidOperationException("Only active events can be expired.");
        Status = EventStatus.Expired;
        AddDomainEvent(new EventExpiredEvent(Id));
    }

    public void Close()
    {
        if (Status != EventStatus.Active)
            throw new InvalidOperationException("Only active events can be closed.");
        Status = EventStatus.Closed;
        AddDomainEvent(new EventClosedEvent(Id));
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}