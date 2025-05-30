namespace AcmeTickets.EventManagement.InternalContracts.Events
{
    public class TicketRequestedEvent
    {
        public int Quantity { get; set; }
        public string? EventId { get; set; }
        public string? CustomerId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
