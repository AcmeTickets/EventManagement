namespace EventManagement.Domain.Events;

public class EventClosedEvent : DomainEvent
{
    public Guid EventId { get; }

    public EventClosedEvent(Guid eventId)
    {
        EventId = eventId;
    }
}
