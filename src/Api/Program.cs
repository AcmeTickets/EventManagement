using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using EventManagement.Application.Services;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Services;
using EventManagement.Domain.Repositories;
using EventManagement.Domain.Services;
using EventManagement.Domain.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Configure NServiceBus
var endpointConfig = ConfigureNServiceBus(builder);
var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfig, builder.Services);

// Register NServiceBus dependencies
builder.Services.AddSingleton<IStartableEndpointWithExternallyManagedContainer>(startableEndpoint);
builder.Services.AddSingleton<IMessageSession>(sp => sp.GetRequiredService<IStartableEndpointWithExternallyManagedContainer>().MessageSession.Value);
builder.Services.AddScoped<ISenderService, NServiceBusEventPublisher>();
builder.Services.AddScoped<IEventService, EventService>();

// Add infrastructure services (e.g., repositories, external services)
DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration, builder.Environment.IsDevelopment());

var app = builder.Build();

// Store the running endpoint instance
IEndpointInstance? runningEndpoint = null;

// Start NServiceBus endpoint when the application starts
app.Lifetime.ApplicationStarted.Register(async () =>
{
    var endpoint = app.Services.GetRequiredService<IStartableEndpointWithExternallyManagedContainer>();
    runningEndpoint = await endpoint.Start(app.Services.GetRequiredService<IServiceProvider>());
});

// Stop NServiceBus endpoint when the application stops
app.Lifetime.ApplicationStopping.Register(async () =>
{
    if (runningEndpoint != null)
    {
        await runningEndpoint.Stop(TimeSpan.FromSeconds(30)); // Specify a graceful stop timeout
    }
});

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();

EndpointConfiguration ConfigureNServiceBus(WebApplicationBuilder builder)
{
    return builder.Environment.IsDevelopment()
        ? NServiceBusConfigurator.DevelopmentConfiguration(
            builder.Configuration,
            "EventManagement.Api",
            routingSettings =>
            {
                routingSettings.RouteToEndpoint(typeof(EventCreatedEvent), "EventManagement.Message");
            })
        : NServiceBusConfigurator.ProductionConfiguration(
            builder.Configuration,
            "EventManagement.Api",
            routingSettings =>
            {
                routingSettings.RouteToEndpoint(typeof(EventCreatedEvent), "EventManagement.Message");
            });
}