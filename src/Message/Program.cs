using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using EventManagement.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using EventManagement.Application.Services;
using EventManagement.Infrastructure.Services;
using EventManagement.Domain.Repositories;
using EventManagement.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing; // For IEndpointRouteBuilder
using Microsoft.Extensions.Diagnostics.HealthChecks; // For HealthCheckOptions
using Microsoft.AspNetCore.Http; // For IResult

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.AddConsole();

// Add ASP.NET Core services for health checks
builder.Services.AddHealthChecks();

// NServiceBus endpoint configuration
var endpointConfig = builder.Environment.IsDevelopment()
    ? NServiceBusConfigurator.DevelopmentConfiguration(
        builder.Configuration,
        "EventManagement.Message",
        routingSettings =>
        {
            // Configure routing for events if needed
        })
    : NServiceBusConfigurator.ProductionConfiguration(
        builder.Configuration,
        "EventManagement.Message",
        routingSettings =>
        {
            // Configure routing for events if needed
        });

// Register NServiceBus dependencies
var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfig, builder.Services);
builder.Services.AddSingleton<IStartableEndpointWithExternallyManagedContainer>(startableEndpoint);
builder.Services.AddSingleton<IMessageSession>(sp => sp.GetRequiredService<IStartableEndpointWithExternallyManagedContainer>().MessageSession.Value);
builder.Services.AddScoped<ISenderService, NServiceBusEventPublisher>();
builder.Services.AddScoped<IEventService, EventService>();

// Register NServiceBus as a hosted service and as a singleton for health check access
builder.Services.AddSingleton<NServiceBusHostedService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<NServiceBusHostedService>());

// Register infrastructure
DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration, builder.Environment.IsDevelopment());

var host = builder.Build();

// Start the health check web server in the background
var nserviceBusHostedService = host.Services.GetRequiredService<NServiceBusHostedService>();
var healthCheckTask = Task.Run(() =>
{
    var healthApp = WebApplication.CreateBuilder().Build();
    healthApp.MapGet("/healthz", () => nserviceBusHostedService.IsHealthy ? Results.Ok("Healthy") : Results.StatusCode(503));
    healthApp.Run("http://0.0.0.0:8080");
});

// Log startup
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting host for endpoint EventManagement.Message");

await host.RunAsync();
await healthCheckTask;

// Hosted service to start/stop NServiceBus endpoint
public class NServiceBusHostedService : IHostedService
{
    private readonly IStartableEndpointWithExternallyManagedContainer _endpoint;
    private readonly ILogger<NServiceBusHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IEndpointInstance? _endpointInstance;
    private bool _isHealthy;

    public NServiceBusHostedService(
        IStartableEndpointWithExternallyManagedContainer endpoint,
        ILogger<NServiceBusHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _endpoint = endpoint;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _isHealthy = false;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting NServiceBus endpoint EventManagement.Message");
        _endpointInstance = await _endpoint.Start(_serviceProvider, cancellationToken);
        _isHealthy = true;
        _logger.LogInformation("NServiceBus endpoint started successfully");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_endpointInstance != null)
        {
            _logger.LogInformation("Stopping NServiceBus endpoint EventManagement.Message");
            await _endpointInstance.Stop();
            _isHealthy = false;
            _logger.LogInformation("NServiceBus endpoint stopped successfully");
        }
    }

    public bool IsHealthy => _isHealthy;
}