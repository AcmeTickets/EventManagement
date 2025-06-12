using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using EventManagement.Domain.Events;


namespace AcmeTickets.Message.Handlers
{
    public class EventCreatedEventHander : IHandleMessages<EventCreatedEvent>
    {
        private readonly ILogger<AddEventHandler> _logger;

        public EventCreatedEventHander(ILogger<AddEventHandler> logger)
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