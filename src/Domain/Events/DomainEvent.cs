namespace EventManagement.Domain.Events;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; protected set; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}
