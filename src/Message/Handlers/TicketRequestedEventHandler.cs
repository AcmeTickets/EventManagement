using System.Threading.Tasks;
using NServiceBus;
using Microsoft.Extensions.Logging;
using AcmeTickets.EventManagement.InternalContracts.Events;

namespace AcmeTickets.Domains.EventManagement.Message.Handlers
{
    public class TicketRequestedEventHandler : IHandleMessages<TicketRequestedEvent>
    {
        private readonly ILogger<TicketRequestedEventHandler> _logger;

        public TicketRequestedEventHandler(ILogger<TicketRequestedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TicketRequestedEvent message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Handled TicketRequestedEvent: {@Event}", message);
            // Add your business logic here
            return Task.CompletedTask;
        }
    }
}