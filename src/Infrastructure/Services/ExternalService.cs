using EventManagement.Domain.Services;
using System.Net.Http;
using System.Text.Json;

namespace EventManagement.Infrastructure.Services;

public class ExternalService : IExternalService
{
    private readonly HttpClient _httpClient;

    public ExternalService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task NotifyEventStatusAsync(Guid eventId, string status, CancellationToken cancellationToken)
    {
        var payload = new { EventId = eventId, Status = status };
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            System.Text.Encoding.UTF8,
            "application/json");

        // Example: Call a third-party API endpoint
        var response = await _httpClient.PostAsync("https://api.example.com/notify", content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}