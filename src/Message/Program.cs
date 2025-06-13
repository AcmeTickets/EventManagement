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

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.AddConsole();

// NServiceBus endpoint configuration
var endpointConfig = builder.Environment.IsDevelopment()
    ? NServiceBusConfigurator.DevelopmentConfiguration(
        builder.Configuration,
        "EventManagement.Message",
        routingSettings =>
        {
            // Example: Configure routing for events if needed
            // routingSettings.RouteToEndpoint(typeof(MyCommand), "MyDestinationEndpoint");
        })
    : NServiceBusConfigurator.ProductionConfiguration(
        builder.Configuration,
        "EventManagement.Message",
        routingSettings =>
        {
            // Example: Configure routing for events if needed
            // routingSettings.RouteToEndpoint(typeof(MyCommand), "MyDestinationEndpoint");
        });

// Create and register NServiceBus endpoint
var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfig, builder.Services);

// Register NServiceBus dependencies
builder.Services.AddSingleton<IStartableEndpointWithExternallyManagedContainer>(startableEndpoint);
builder.Services.AddSingleton<IMessageSession>(sp => sp.GetRequiredService<IStartableEndpointWithExternallyManagedContainer>().MessageSession.Value);
builder.Services.AddScoped<ISenderService, NServiceBusEventPublisher>();
builder.Services.AddScoped<IEventService, EventService>();

// Register NServiceBus as a hosted service
builder.Services.AddHostedService<NServiceBusHostedService>();

// Register infrastructure
DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration, builder.Environment.IsDevelopment());

var host = builder.Build();

// Log startup
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting host for endpoint EventManagement.Message");

await host.RunAsync();

// Hosted service to start/stop NServiceBus endpoint
public class NServiceBusHostedService : IHostedService
{
    private readonly IStartableEndpointWithExternallyManagedContainer _endpoint;
    private readonly ILogger<NServiceBusHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IEndpointInstance? _endpointInstance;

    public NServiceBusHostedService(
        IStartableEndpointWithExternallyManagedContainer endpoint,
        ILogger<NServiceBusHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _endpoint = endpoint;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting NServiceBus endpoint EventManagement.Message");
        _endpointInstance = await _endpoint.Start(_serviceProvider, cancellationToken);
        _logger.LogInformation("NServiceBus endpoint started successfully");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_endpointInstance != null)
        {
            _logger.LogInformation("Stopping NServiceBus endpoint EventManagement.Message");
            await _endpointInstance.Stop();
            _logger.LogInformation("NServiceBus endpoint stopped successfully");
        }
    }
}