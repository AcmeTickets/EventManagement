namespace EventManagement.Domain.Events;

public class EventExpiredEvent : DomainEvent
{
    public Guid EventId { get; }

    public EventExpiredEvent(Guid eventId)
    {
        EventId = eventId;
    }
}
