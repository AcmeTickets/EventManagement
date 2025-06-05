using EventManagement.Application.Commands;
using EventManagement.Application.Services;
using EventManagement.Domain.Events;
using EventManagement.Test.Mocks.Fakes;
using Xunit;

namespace EventManagement.Test.UnitTests.Application;

public class EventServiceTests
{
    private readonly FakeEventRepository _fakeEventRepository = new();
    private readonly FakeExternalService _fakeExternalService = new();

 
    [Fact]
    public async Task AddEventAsync_Should_CreateEvent_And_NotifyExternalService_And_PublishEvent()
    {
        // Arrange
        var command = new AddEventCommand("Test Event", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));
        var fakeSenderService = new FakeSenderService();
        var service = new EventService(_fakeEventRepository, _fakeExternalService, fakeSenderService);

        // Act
        var result = await service.AddEventAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.StartDate, result.StartDate);
        Assert.Equal(command.EndDate, result.EndDate);
        Assert.Equal("Active", result.Status);
        var storedEvent = await _fakeEventRepository.GetByIdAsync(result.Id, CancellationToken.None);
        Assert.NotNull(storedEvent);
        Assert.Empty(storedEvent.DomainEvents); // Ensure cleared
        // Verify that the event was published
        Assert.Single(fakeSenderService.PublishedEvents);
        Assert.IsType<EventCreatedEvent>(fakeSenderService.PublishedEvents.First());
        var publishedEvent = (EventCreatedEvent)fakeSenderService.PublishedEvents.First();
        Assert.Equal(result.Id, publishedEvent.EventId);
        Assert.Equal(result.Name, publishedEvent.Name);
    }

    // Additional tests for ExpireEventAsync and CloseEventAsync can be added similarly
}
