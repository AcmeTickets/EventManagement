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
        Assert.Single(evt.DomainEvents);
        Assert.IsType<EventExpiredEvent>(evt.DomainEvents.First());
        var domainEvent = (EventExpiredEvent)evt.DomainEvents.First();
        Assert.Equal(evt.Id, domainEvent.EventId);
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
        Assert.Single(evt.DomainEvents);
        Assert.IsType<EventClosedEvent>(evt.DomainEvents.First());
        var domainEvent = (EventClosedEvent)evt.DomainEvents.First();
        Assert.Equal(evt.Id, domainEvent.EventId);
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
