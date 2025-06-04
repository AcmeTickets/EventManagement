namespace EventManagement.Domain.Events;

public class EventCreatedEvent : DomainEvent
{
    public Guid EventId { get; }
    public string Name { get; }

    public EventCreatedEvent(Guid eventId, string name)
    {
        EventId = eventId;
        Name = name;
    }
}