using Microsoft.AspNetCore.Mvc;
using AcmeTickets.EventManagement.InternalContracts.Events;
using EventManagement.Application.Commands;
using EventManagement.Application.DTOs;
using EventManagement.Application.Services;

namespace AcmeTickets.Domains.EventManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        private readonly IEventService _eventService;

        public TicketController(ILogger<TicketController> logger, IEventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        [HttpPost]
        public async Task<ActionResult<EventDto>> AddEvent([FromBody] AddEventCommand command)
        {
            var result = await _eventService.AddEventAsync(command, CancellationToken.None);
            return CreatedAtAction(nameof(GetEvent), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(Guid id)
        {
            // Placeholder: Implement if needed
            await Task.CompletedTask;
            return NotFound();
        }

        [HttpPost("{id}/expire")]
        public async Task<IActionResult> ExpireEvent(Guid id)
        {
            await _eventService.ExpireEventAsync(new ExpireEventCommand(id), CancellationToken.None);
            return NoContent();
        }

        [HttpPost("{id}/close")]
        public async Task<IActionResult> CloseEvent(Guid id)
        {
            await _eventService.CloseEventAsync(new CloseEventCommand(id), CancellationToken.None);
            return NoContent();
        }

        // [HttpPost]
        // public async Task<IActionResult> RequestTickets([FromBody] TicketRequest request, [FromServices] NServiceBus.IMessageSession messageSession)
        // {
        //     _logger.LogInformation("Received ticket request: {@Request}", request);
        //     var evt = new TicketRequestedEvent
        //     {
        //         Quantity = request.Quantity,
        //         EventId = request.EventId,
        //         CustomerId = request.CustomerId,
        //         RequestedAt = DateTime.UtcNow
        //     };
        //     await messageSession.Publish(evt);
        //     return Ok(new { message = "Ticket request received and event published." });
        // }
    }

    // public class TicketRequest
    // {
    //     public int Quantity { get; set; }
    //     public string? EventId { get; set; }
    //     public string? CustomerId { get; set; }
    // }
}
