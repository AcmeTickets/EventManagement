using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using EventManagement.Domain.Events;


namespace AcmeTickets.Message.Handlers
{
    public class EventCreatedEventHandler : IHandleMessages<EventCreatedEvent>
    {
        private readonly ILogger<EventCreatedEventHandler> _logger;

        public EventCreatedEventHandler(ILogger<EventCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EventCreatedEvent message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Handled EventCreatedEvent command: {@AddEvent}", message);
            return Task.CompletedTask;
        }
    }
}