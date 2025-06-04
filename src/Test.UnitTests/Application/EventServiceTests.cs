using EventManagement.Application.Commands;
using EventManagement.Application.Services;
using EventManagement.Test.Mocks.Fakes;
using Xunit;

namespace EventManagement.Test.UnitTests.Application;

public class EventServiceTests
{
    private readonly FakeEventRepository _fakeEventRepository = new();
    private readonly FakeExternalService _fakeExternalService = new();

    [Fact]
    public async Task AddEventAsync_Should_CreateEvent_And_NotifyExternalService()
    {
        // Arrange
        var command = new AddEventCommand("Test Event", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));
        var service = new EventService(_fakeEventRepository, _fakeExternalService, null);

        // Act
        var result = await service.AddEventAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.StartDate, result.StartDate);
        Assert.Equal(command.EndDate, result.EndDate);
        Assert.Equal("Active", result.Status);
        Assert.Single(_fakeExternalService.Notifications);
        Assert.Equal(result.Id, _fakeExternalService.Notifications[0].EventId);
        Assert.Equal("Created", _fakeExternalService.Notifications[0].Status);
        var storedEvent = await _fakeEventRepository.GetByIdAsync(result.Id, CancellationToken.None);
        Assert.NotNull(storedEvent);
        Assert.Empty(storedEvent.DomainEvents); // Ensure cleared
    }

    // Additional tests for ExpireEventAsync and CloseEventAsync can be added similarly
}
