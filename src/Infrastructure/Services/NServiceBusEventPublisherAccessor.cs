    using EventManagement.Application.Services;

namespace EventManagement.Infrastructure.Services
{
    public static class NServiceBusEventPublisherAccessor
    {
        public static ISenderService? Instance { get; set; }
    }
}
