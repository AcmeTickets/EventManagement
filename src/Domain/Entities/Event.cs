using EventManagement.Domain.Events;

namespace EventManagement.Domain.Entities;

public class Event
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public EventStatus Status { get; private set; }
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