using EventManagement.Domain.Entities;
using EventManagement.Domain.Events;
using Xunit;

namespace EventManagement.Test.UnitTests.Domain;

public class EventTests
{
    [Fact]
    public void Create_Should_SetProperties_And_RaiseEventCreatedEvent()
    {
        // Arrange
        var name = "Test Event";
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(2);

        // Act
        var evt = Event.Create(name, startDate, endDate);

        // Assert
        Assert.NotEqual(Guid.Empty, evt.Id);
        Assert.Equal(name, evt.Name);
        Assert.Equal(startDate, evt.StartDate);
        Assert.Equal(endDate, evt.EndDate);
        Assert.Equal(EventStatus.Active, evt.Status);
        Assert.Single(evt.DomainEvents);
        Assert.IsType<EventCreatedEvent>(evt.DomainEvents.First());
        var domainEvent = (EventCreatedEvent)evt.DomainEvents.First();
        Assert.Equal(evt.Id, domainEvent.EventId);
        Assert.Equal(name, domainEvent.Name);
    }

    [Fact]
    public void Expire_Should_ChangeStatus_And_RaiseEventExpiredEvent()
    {
        // Arrange
        var evt = Event.Create("Test Event", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));

        // Act
        evt.Expire();

        // Assert
        Assert.Equal(EventStatus.Expired, evt.Status);
        Assert.Equal(2, evt.DomainEvents.Count);
        Assert.Contains(evt.DomainEvents, e => e is EventCreatedEvent);
        Assert.Contains(evt.DomainEvents, e => e is EventExpiredEvent);
        var expiredEvent = (EventExpiredEvent)evt.DomainEvents.Last(e => e is EventExpiredEvent);
        Assert.Equal(evt.Id, expiredEvent.EventId);
    }

    [Fact]
    public void Expire_When_NotActive_Should_ThrowException()
    {
        // Arrange
        var evt = Event.Create("Test Event", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));
        evt.Expire();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => evt.Expire());
    }

    [Fact]
    public void Close_Should_ChangeStatus_And_RaiseEventClosedEvent()
    {
        // Arrange
        var evt = Event.Create("Test Event", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));

        // Act
        evt.Close();

        // Assert
        Assert.Equal(EventStatus.Closed, evt.Status);
        Assert.Equal(2, evt.DomainEvents.Count);
        Assert.Contains(evt.DomainEvents, e => e is EventCreatedEvent);
        Assert.Contains(evt.DomainEvents, e => e is EventClosedEvent);
        var closedEvent = (EventClosedEvent)evt.DomainEvents.Last(e => e is EventClosedEvent);
        Assert.Equal(evt.Id, closedEvent.EventId);
    }

    [Fact]
    public void Close_When_NotActive_Should_ThrowException()
    {
        // Arrange
        var evt = Event.Create("Test Event", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));
        evt.Close();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => evt.Close());
    }
}
