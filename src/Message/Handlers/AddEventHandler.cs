using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using AcmeTickets.Contracts.Public.Platform.Commands;

namespace AcmeTickets.Message.Handlers
{
    public class AddEventHandler : IHandleMessages<AddEvent>
    {
        private readonly ILogger<AddEventHandler> _logger;

        public AddEventHandler(ILogger<AddEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(AddEvent message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Handled AddEvent command: {@AddEvent}", message);
            return Task.CompletedTask;
        }
    }
}