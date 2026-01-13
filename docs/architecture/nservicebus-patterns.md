# NServiceBus Patterns

## Overview

NServiceBus is the messaging framework that enables event-driven architecture in this platform. It provides **message routing**, **saga orchestration**, **retry policies**, **distributed transactions**, and **publish/subscribe** patterns.

## Core Concepts

### Messages
Base unit of communication. Three types:
- **Commands**: Tell a service to do something (unicast, one receiver)
- **Events**: Notify that something happened (broadcast, many receivers)
- **Sagas**: Long-running stateful workflows

---

## Commands

### Definition
A command is an **instruction** sent to a specific endpoint to perform an action.

**Characteristics**:
- **Imperative naming**: `RegisterBeneficiaryCommand`, `ValidateBeneficiaryCommand`
- **Unicast**: Sent to exactly one endpoint
- **Must succeed**: Failures are exceptional
- **Not published**: Use `context.Send()` or `context.SendLocal()`

### Command Structure
```csharp
// Beneficiary/Domain/Contracts/Commands/RegisterBeneficiaryCommand.cs
public class RegisterBeneficiaryCommand
{
    public Guid BeneficiaryId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Nationality { get; set; }
}
```

**Best Practices**:
- Use imperative verb (Register, Validate, Update, Delete)
- Include all data needed to execute
- Don't include unnecessary context
- Validate in handler, not in command

### Command Handler
```csharp
// Beneficiary/Endpoint.In/Handlers/Commands/RegisterBeneficiaryCommandHandler.cs
public class RegisterBeneficiaryCommandHandler : 
    IHandleMessages<RegisterBeneficiaryCommand>
{
    private readonly IBeneficiaryRepository _repository;
    private readonly ILogger<RegisterBeneficiaryCommandHandler> _logger;
    
    public RegisterBeneficiaryCommandHandler(
        IBeneficiaryRepository repository,
        ILogger<RegisterBeneficiaryCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task Handle(
        RegisterBeneficiaryCommand message, 
        IMessageHandlerContext context)
    {
        _logger.LogInformation(
            "Registering beneficiary: {FirstName} {LastName}", 
            message.FirstName, 
            message.LastName);
        
        // 1. Validate
        if (string.IsNullOrWhiteSpace(message.FirstName))
            throw new ValidationException("FirstName is required");
        
        // 2. Create entity
        var beneficiary = new Beneficiary
        {
            Id = message.BeneficiaryId,
            FirstName = message.FirstName,
            LastName = message.LastName,
            DateOfBirth = message.DateOfBirth,
            Nationality = message.Nationality,
            CaseStatus = CaseStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        
        // 3. Save to repository
        await _repository.SaveAsync(beneficiary);
        
        // 4. Publish event (notify other domains)
        await context.Publish(new BeneficiaryRegisteredEvent
        {
            BeneficiaryId = beneficiary.Id,
            FirstName = beneficiary.FirstName,
            LastName = beneficiary.LastName,
            RegisteredAt = beneficiary.CreatedAt
        });
        
        _logger.LogInformation(
            "Beneficiary registered: {BeneficiaryId}", 
            beneficiary.Id);
    }
}
```

### Sending Commands

#### From API (Azure Function)
```csharp
// Beneficiary/Api/BeneficiaryRegistrationFunction.cs
[Function("RegisterBeneficiary")]
public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
{
    var dto = await req.ReadFromJsonAsync<BeneficiaryRegistrationDto>();
    
    // Send command to message bus
    await _messageSession.Send(new RegisterBeneficiaryCommand
    {
        BeneficiaryId = Guid.NewGuid(),
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        DateOfBirth = dto.DateOfBirth,
        Nationality = dto.Nationality
    });
    
    var response = req.CreateResponse(HttpStatusCode.Accepted);
    return response;
}
```

#### From Another Handler
```csharp
public async Task Handle(BulkUploadStartedEvent message, IMessageHandlerContext context)
{
    foreach (var row in message.Rows)
    {
        // Send command for each row
        await context.SendLocal(new ValidateBeneficiaryCommand
        {
            UploadId = message.UploadId,
            RowNumber = row.RowNumber,
            Data = row.Data
        });
    }
}
```

#### SendLocal vs Send
- **SendLocal**: Send to same endpoint (local processing)
- **Send**: Send to different endpoint (remote processing)

```csharp
// Send to same endpoint
await context.SendLocal(new ValidateBeneficiaryCommand { /* */ });

// Send to specific endpoint
await context.Send("Beneficiary.Endpoint.In", new RegisterBeneficiaryCommand { /* */ });
```

---

## Events

### Definition
An event notifies that something **has happened**. Multiple subscribers can react independently.

**Characteristics**:
- **Past tense naming**: `BeneficiaryRegisteredEvent`, `PointsAwardedEvent`
- **Multicast**: Many endpoints can subscribe
- **Fire and forget**: Publisher doesn't care who subscribes
- **Published**: Use `context.Publish()`

### Event Handler
```csharp
// Points/Endpoint.In/Handlers/Events/BeneficiaryRegisteredEventHandler.cs
public class BeneficiaryRegisteredEventHandler : 
    IHandleMessages<BeneficiaryRegisteredEvent>
{
    private readonly IPointsRepository _repository;
    
    public async Task Handle(
        BeneficiaryRegisteredEvent message, 
        IMessageHandlerContext context)
    {
        // React to event: award welcome points
        var pointsAccount = new PointsAccount
        {
            Id = Guid.NewGuid(),
            UserId = message.BeneficiaryId,
            CurrentBalance = 100,
            LifetimePoints = 100
        };
        
        await _repository.SaveAsync(pointsAccount);
        
        // Publish own event
        await context.Publish(new PointsAwardedEvent
        {
            UserId = message.BeneficiaryId,
            Points = 100,
            Reason = "Welcome bonus",
            AwardedAt = DateTime.UtcNow
        });
    }
}
```

### Publishing Events
```csharp
// Publish event (all subscribers receive it)
await context.Publish(new BeneficiaryRegisteredEvent
{
    BeneficiaryId = beneficiary.Id,
    FirstName = beneficiary.FirstName,
    LastName = beneficiary.LastName
});
```

---

## Sagas

### Definition
A saga is a **long-running stateful workflow** that coordinates multiple messages.

**Characteristics**:
- Maintains state across multiple messages
- Can timeout and compensate
- Stores data in persistence (CosmosDB, SQL Server, etc.)
- Completes when workflow finishes

### When to Use Sagas
Use sagas for:
- **Multi-step workflows**: Bulk upload (validate → save → notify)
- **Compensation logic**: Payment failed → refund → notify
- **Timeouts**: Waiting for external system response
- **Correlation**: Track related messages (all rows in same upload)

Don't use sagas for:
- **Simple event reactions**: Just use a handler
- **Stateless processing**: No need to track progress
- **Single message processing**: Handlers are sufficient

### Saga Structure
```csharp
// Beneficiary/Endpoint.In/Sagas/BulkBeneficiaryUploadSaga.cs
public class BulkBeneficiaryUploadSaga : 
    Saga<BulkBeneficiaryUploadSagaData>,
    IAmStartedByMessages<BulkUploadStartedEvent>,
    IHandleMessages<BeneficiaryValidatedEvent>,
    IHandleMessages<BeneficiaryValidationFailedEvent>,
    IHandleTimeouts<BulkUploadTimeout>
{
    private readonly ILogger<BulkBeneficiaryUploadSaga> _logger;
    
    public BulkBeneficiaryUploadSaga(ILogger<BulkBeneficiaryUploadSaga> logger)
    {
        _logger = logger;
    }
    
    // Configure correlation (how messages map to saga instances)
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BulkBeneficiaryUploadSagaData> mapper)
    {
        mapper.MapSaga(saga => saga.UploadId)
            .ToMessage<BulkUploadStartedEvent>(msg => msg.UploadId)
            .ToMessage<BeneficiaryValidatedEvent>(msg => msg.UploadId)
            .ToMessage<BeneficiaryValidationFailedEvent>(msg => msg.UploadId);
    }
    
    // Saga starts when this message arrives
    public async Task Handle(BulkUploadStartedEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Starting bulk upload: {UploadId}", message.UploadId);
        
        Data.UploadId = message.UploadId;
        Data.TotalRecords = message.TotalRecords;
        Data.ProcessedRecords = 0;
        Data.SuccessCount = 0;
        Data.ErrorCount = 0;
        Data.StartedAt = DateTime.UtcNow;
        
        // Set timeout (in case processing hangs)
        await RequestTimeout<BulkUploadTimeout>(context, TimeSpan.FromMinutes(30));
        
        // Send validation command for each row
        foreach (var row in message.Rows)
        {
            await context.SendLocal(new ValidateBeneficiaryCommand
            {
                UploadId = message.UploadId,
                RowNumber = row.RowNumber,
                Data = row.Data
            });
        }
    }
    
    // Handle successful validation
    public async Task Handle(BeneficiaryValidatedEvent message, IMessageHandlerContext context)
    {
        Data.ProcessedRecords++;
        Data.SuccessCount++;
        
        _logger.LogInformation(
            "Validated row {RowNumber}, Progress: {Processed}/{Total}", 
            message.RowNumber, 
            Data.ProcessedRecords, 
            Data.TotalRecords);
        
        await CheckCompletion(context);
    }
    
    // Handle validation failure
    public async Task Handle(BeneficiaryValidationFailedEvent message, IMessageHandlerContext context)
    {
        Data.ProcessedRecords++;
        Data.ErrorCount++;
        Data.Errors.Add(new ValidationError
        {
            RowNumber = message.RowNumber,
            ErrorMessage = message.ErrorMessage
        });
        
        _logger.LogWarning(
            "Validation failed for row {RowNumber}: {Error}", 
            message.RowNumber, 
            message.ErrorMessage);
        
        await CheckCompletion(context);
    }
    
    // Handle timeout
    public async Task Timeout(BulkUploadTimeout state, IMessageHandlerContext context)
    {
        _logger.LogError("Bulk upload timed out: {UploadId}", Data.UploadId);
        
        await context.Publish(new BulkUploadFailedEvent
        {
            UploadId = Data.UploadId,
            Reason = "Processing timeout"
        });
        
        MarkAsComplete();
    }
    
    // Check if all records processed
    private async Task CheckCompletion(IMessageHandlerContext context)
    {
        if (Data.ProcessedRecords >= Data.TotalRecords)
        {
            _logger.LogInformation(
                "Bulk upload completed: {UploadId}, Success: {Success}, Errors: {Errors}", 
                Data.UploadId, 
                Data.SuccessCount, 
                Data.ErrorCount);
            
            // Publish completion event
            await context.Publish(new BulkUploadCompletedEvent
            {
                UploadId = Data.UploadId,
                TotalRecords = Data.TotalRecords,
                SuccessCount = Data.SuccessCount,
                ErrorCount = Data.ErrorCount,
                Errors = Data.Errors,
                CompletedAt = DateTime.UtcNow
            });
            
            // Mark saga as complete (deletes saga data)
            MarkAsComplete();
        }
    }
}
```

### Saga Data
```csharp
// Beneficiary/Endpoint.In/Sagas/BulkBeneficiaryUploadSagaData.cs
public class BulkBeneficiaryUploadSagaData : ContainSagaData
{
    public Guid UploadId { get; set; }  // Correlation ID
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public DateTime StartedAt { get; set; }
}

public class ValidationError
{
    public int RowNumber { get; set; }
    public string ErrorMessage { get; set; }
}
```

**Saga Data Rules**:
- Inherits from `ContainSagaData` (provides Id, Originator, OriginalMessageId)
- Must be serializable (plain properties, no methods)
- Stored in persistence (CosmosDB table, SQL Server, etc.)
- Deleted when saga completes

### Saga Correlation

**Correlation** maps incoming messages to the correct saga instance.

```csharp
protected override void ConfigureHowToFindSaga(
    SagaPropertyMapper<BulkBeneficiaryUploadSagaData> mapper)
{
    // Map saga property (UploadId) to message property (UploadId)
    mapper.MapSaga(saga => saga.UploadId)
        .ToMessage<BulkUploadStartedEvent>(msg => msg.UploadId)
        .ToMessage<BeneficiaryValidatedEvent>(msg => msg.UploadId)
        .ToMessage<BeneficiaryValidationFailedEvent>(msg => msg.UploadId);
}
```

**Flow**:
1. `BulkUploadStartedEvent` arrives with `UploadId = 123`
2. NServiceBus checks: Does saga exist with `UploadId = 123`?
3. No → Create new saga instance
4. Yes → Route message to existing saga instance

### Saga Timeouts

Sagas can request timeouts to handle delays:

```csharp
// Request timeout
await RequestTimeout<BulkUploadTimeout>(context, TimeSpan.FromMinutes(30));

// Handle timeout
public async Task Timeout(BulkUploadTimeout state, IMessageHandlerContext context)
{
    _logger.LogError("Upload timed out: {UploadId}", Data.UploadId);
    
    await context.Publish(new BulkUploadFailedEvent
    {
        UploadId = Data.UploadId,
        Reason = "Timeout"
    });
    
    MarkAsComplete();
}

// Timeout message
public class BulkUploadTimeout { }
```

**Use Cases**:
- Waiting for external system response
- Max processing time limit
- Scheduled actions (e.g., "Send reminder in 24 hours")

### Completing Sagas

```csharp
// Mark saga as complete
MarkAsComplete();
```

**When to Complete**:
- Workflow successfully finished
- Workflow failed and compensated
- Timeout exceeded

**What Happens**:
- Saga data deleted from persistence
- Saga instance removed from memory
- Future messages with same correlation ID create new saga

---

## Message Handlers

### Handler Lifecycle
```csharp
public class MyHandler : IHandleMessages<MyCommand>
{
    private readonly IRepository _repository;
    
    // Constructor injection
    public MyHandler(IRepository repository)
    {
        _repository = repository;
    }
    
    // Handle method (required)
    public async Task Handle(MyCommand message, IMessageHandlerContext context)
    {
        // Process message
    }
}
```

**Lifecycle**:
1. Message arrives on queue
2. NServiceBus deserializes message
3. NServiceBus creates handler instance (via DI)
4. Calls `Handle()` method
5. If success, removes message from queue
6. If exception, retries (immediate → delayed → error queue)

### Multiple Handlers for Same Message

**Each domain can have its own handler**:

```csharp
// Points Domain
public class BeneficiaryRegisteredEventHandler : IHandleMessages<BeneficiaryRegisteredEvent>
{
    public async Task Handle(BeneficiaryRegisteredEvent message, IMessageHandlerContext context)
    {
        // Award points
    }
}

// Medical Domain
public class BeneficiaryRegisteredEventHandler : IHandleMessages<BeneficiaryRegisteredEvent>
{
    public async Task Handle(BeneficiaryRegisteredEvent message, IMessageHandlerContext context)
    {
        // Create medical case
    }
}

// Platform SignalR
public class SignalRBeneficiaryRegisteredHandler : IHandleMessages<BeneficiaryRegisteredEvent>
{
    public async Task Handle(BeneficiaryRegisteredEvent message, IMessageHandlerContext context)
    {
        // Send notification to UI
    }
}
```

**Each handler runs independently in its own endpoint.**

### Handler Ordering

Within the same endpoint, you can control handler order:

```csharp
// First handler
public class LoggingHandler : IHandleMessages<MyCommand>
{
    public async Task Handle(MyCommand message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Processing command");
    }
}

// Second handler (executes after LoggingHandler)
[HandlerOrder(After = typeof(LoggingHandler))]
public class BusinessLogicHandler : IHandleMessages<MyCommand>
{
    public async Task Handle(MyCommand message, IMessageHandlerContext context)
    {
        // Business logic
    }
}
```

**Prefer**: Single handler per message type per endpoint. Use sagas for complex orchestration.

---

## Endpoint Configuration

### Endpoint Structure

Each Azure Functions project is an NServiceBus endpoint:

```
Beneficiary/
└── src/
    ├── Endpoint.In/          ← NServiceBus endpoint (receives messages)
    ├── Endpoint.Out/         ← NServiceBus endpoint (sends messages) [optional]
    └── Api/                  ← Azure Functions HTTP triggers
```

### Endpoint.In Configuration

```csharp
// Beneficiary/Endpoint.In/Program.cs
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .UseNServiceBus("Beneficiary.Endpoint.In", (context, configuration) =>
    {
        // 1. Transport (Azure Service Bus)
        var transport = configuration.UseTransport<AzureServiceBusTransport>();
        transport.ConnectionString(context.Configuration["ServiceBus:ConnectionString"]);
        
        // 2. Persistence (CosmosDB for sagas)
        var persistence = configuration.UsePersistence<CosmosPersistence>();
        persistence.CosmosClient(new CosmosClient(
            context.Configuration["CosmosDb:ConnectionString"]));
        persistence.DatabaseName("BeneficiaryDb");
        persistence.DefaultContainer("Sagas", "/id");
        
        // 3. Serialization
        configuration.UseSerialization<NewtonsoftJsonSerializer>();
        
        // 4. Recoverability (retry policy)
        configuration.Recoverability()
            .Immediate(immediate => immediate.NumberOfRetries(3))
            .Delayed(delayed => delayed
                .NumberOfRetries(5)
                .TimeIncrease(TimeSpan.FromMinutes(1)));
        
        // 5. Routing (where to send commands)
        var routing = transport.Routing();
        routing.RouteToEndpoint(
            typeof(RegisterBeneficiaryCommand), 
            "Beneficiary.Endpoint.In");
        
        // 6. Enable installers (create queues automatically)
        configuration.EnableInstallers();
    })
    .ConfigureServices(services =>
    {
        // Register dependencies
        services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
        services.AddLogging();
    })
    .Build();

await host.RunAsync();
```

### API Configuration (Send Messages)

```csharp
// Beneficiary/Api/Program.cs
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .UseNServiceBus(configuration =>
    {
        // Send-only endpoint (doesn't receive messages)
        configuration.AdvancedConfiguration.SendOnly();
        
        // Transport
        var transport = configuration.AdvancedConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.ConnectionString(Environment.GetEnvironmentVariable("ServiceBus:ConnectionString"));
        
        // Routing
        var routing = transport.Routing();
        routing.RouteToEndpoint(
            typeof(RegisterBeneficiaryCommand), 
            "Beneficiary.Endpoint.In");
    })
    .ConfigureServices(services =>
    {
        services.AddLogging();
    })
    .Build();

await host.RunAsync();
```

**Send-Only Endpoint**:
- Sends commands/events
- Doesn't process incoming messages
- Doesn't need handlers or sagas
- Doesn't need persistence

---

## Message Routing

### Routing Commands

Commands must be explicitly routed:

```csharp
var routing = transport.Routing();

// Route specific command type
routing.RouteToEndpoint(
    typeof(RegisterBeneficiaryCommand), 
    "Beneficiary.Endpoint.In");

// Route multiple commands
routing.RouteToEndpoint(
    assembly: typeof(RegisterBeneficiaryCommand).Assembly,
    destination: "Beneficiary.Endpoint.In");
```

### Publishing Events

Events don't need routing - subscribers register themselves:

```csharp
// Publisher (Beneficiary domain)
await context.Publish(new BeneficiaryRegisteredEvent { /* */ });

// Subscriber (Points domain) - automatically receives it
public class BeneficiaryRegisteredEventHandler : IHandleMessages<BeneficiaryRegisteredEvent>
{
    // Handles event
}
```

NServiceBus automatically creates subscriptions when endpoint starts.

---

## Retry Policies

### Immediate Retries
Fast retries with no delay (transient errors):

```csharp
configuration.Recoverability()
    .Immediate(immediate => immediate.NumberOfRetries(3));
```

**Use For**: Network blips, momentary unavailability

### Delayed Retries
Slower retries with increasing delay:

```csharp
configuration.Recoverability()
    .Delayed(delayed => delayed
        .NumberOfRetries(5)
        .TimeIncrease(TimeSpan.FromMinutes(1)));
```

**Use For**: Database deadlocks, rate limiting, external service outages

**Flow**:
```
1st attempt → fail → immediate retry (3x)
Still failing → delayed retry #1 (1 min delay)
Still failing → delayed retry #2 (2 min delay)
Still failing → delayed retry #3 (3 min delay)
Still failing → delayed retry #4 (4 min delay)
Still failing → delayed retry #5 (5 min delay)
Still failing → move to error queue
```

### Error Queue

Messages that fail all retries go to error queue:

```
Beneficiary.Endpoint.In.error
```

**Recovery**:
1. Fix underlying issue (bug, infrastructure, etc.)
2. Replay messages from error queue (ServicePulse or code)

```csharp
// Manually replay from error queue
await context.Send(new RetryMessage
{
    MessageId = failedMessageId
});
```

---

## Transactions

### Default Behavior: Receive Transaction
```csharp
// Handler
public async Task Handle(RegisterBeneficiaryCommand message, IMessageHandlerContext context)
{
    // 1. Save to database
    await _repository.SaveAsync(beneficiary);
    
    // 2. Publish event
    await context.Publish(new BeneficiaryRegisteredEvent { /* */ });
}
```

**If handler fails**:
- Database save rolled back
- Event not published
- Message returned to queue for retry

**If handler succeeds**:
- Database save committed
- Event published
- Message removed from queue

### Distributed Transactions

For operations across multiple resources:

```csharp
configuration.EnableOutbox();
```

**Outbox Pattern**:
1. Save entity + outgoing messages to same database (atomic)
2. Background process sends messages from outbox table
3. Guarantees exactly-once delivery

**Example**:
```csharp
// Save beneficiary + publish event (atomic)
await _repository.SaveAsync(beneficiary);
await context.Publish(new BeneficiaryRegisteredEvent { /* */ });

// Both succeed or both fail (no partial state)
```

---

## Serialization

### Default: JSON Serialization

```csharp
configuration.UseSerialization<NewtonsoftJsonSerializer>();
```

**Message**:
```json
{
  "BeneficiaryId": "123e4567-e89b-12d3-a456-426614174000",
  "FirstName": "John",
  "LastName": "Doe",
  "DateOfBirth": "1990-01-01T00:00:00Z"
}
```

### Custom Serialization Settings

```csharp
var serialization = configuration.UseSerialization<NewtonsoftJsonSerializer>();
serialization.Settings(new JsonSerializerSettings
{
    DateFormatHandling = DateFormatHandling.IsoDateFormat,
    NullValueHandling = NullValueHandling.Ignore,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
});
```

---

## Testing Handlers

### Unit Testing

```csharp
public class RegisterBeneficiaryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_SavesBeneficiary()
    {
        // Arrange
        var repository = new Mock<IBeneficiaryRepository>();
        var handler = new RegisterBeneficiaryCommandHandler(repository.Object);
        var context = new TestableMessageHandlerContext();
        
        var command = new RegisterBeneficiaryCommand
        {
            BeneficiaryId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
        };
        
        // Act
        await handler.Handle(command, context);
        
        // Assert
        repository.Verify(r => r.SaveAsync(It.IsAny<Beneficiary>()), Times.Once);
        Assert.Single(context.PublishedMessages);
        Assert.IsType<BeneficiaryRegisteredEvent>(context.PublishedMessages[0].Message);
    }
}
```

### Integration Testing

```csharp
[Fact]
public async Task EndToEnd_RegisterBeneficiary()
{
    // Start endpoint
    var endpoint = await Endpoint.Start(endpointConfiguration);
    
    // Send command
    await endpoint.Send(new RegisterBeneficiaryCommand
    {
        BeneficiaryId = Guid.NewGuid(),
        FirstName = "John",
        LastName = "Doe"
    });
    
    // Wait for processing
    await Task.Delay(1000);
    
    // Verify beneficiary saved
    var beneficiary = await _repository.GetByIdAsync(beneficiaryId);
    Assert.NotNull(beneficiary);
    
    await endpoint.Stop();
}
```

---

## Best Practices

### 1. Command Naming
```csharp
// Good
RegisterBeneficiaryCommand
ValidateBeneficiaryCommand
UpdateBeneficiaryStatusCommand

// Bad
Beneficiary (too vague)
RegisteredBeneficiaryCommand (sounds like event)
DoBeneficiaryRegistration (awkward)
```

### 2. Event Naming
```csharp
// Good
BeneficiaryRegisteredEvent
PointsAwardedEvent
CaseClosedEvent

// Bad
RegisterBeneficiaryEvent (sounds like command)
BeneficiaryRegistration (missing Event suffix)
OnBeneficiaryRegistered (don't use "On" prefix)
```

### 3. Handler Responsibility
```csharp
// Good - focused handler
public class RegisterBeneficiaryCommandHandler : IHandleMessages<RegisterBeneficiaryCommand>
{
    public async Task Handle(RegisterBeneficiaryCommand message, IMessageHandlerContext context)
    {
        // 1. Validate
        // 2. Save
        // 3. Publish event
    }
}

// Bad - handler doing too much
public class RegisterBeneficiaryCommandHandler : IHandleMessages<RegisterBeneficiaryCommand>
{
    public async Task Handle(RegisterBeneficiaryCommand message, IMessageHandlerContext context)
    {
        // Save beneficiary
        // Award points (should be in Points domain)
        // Send email (should be in Notifications domain)
        // Update UI (should be SignalR handler)
    }
}
```

### 4. Saga Usage
```csharp
// Use saga for: Multi-step workflow
public class BulkUploadSaga : Saga<BulkUploadSagaData>,
    IAmStartedByMessages<BulkUploadStartedEvent>,
    IHandleMessages<RowProcessedEvent>
{
    // Tracks progress across many messages
}

// Don't use saga for: Simple event reaction
public class BeneficiaryRegisteredEventHandler : IHandleMessages<BeneficiaryRegisteredEvent>
{
    // Just award points, no saga needed
}
```

### 5. Idempotency
```csharp
// Good - idempotent handler
public async Task Handle(PointsAwardedEvent message, IMessageHandlerContext context)
{
    var existing = await _repository.GetTransactionByIdAsync(message.TransactionId);
    if (existing != null)
        return;  // Already processed
        
    // Process points
}

// Bad - not idempotent (duplicate processing)
public async Task Handle(PointsAwardedEvent message, IMessageHandlerContext context)
{
    account.Balance += message.Points;  // Duplicate if retried!
    await _repository.SaveAsync(account);
}
```

---

## Troubleshooting

### Messages Not Being Received
**Check**:
1. Endpoint configured correctly (`UseNServiceBus()`)
2. Handler registered (in correct assembly)
3. Message routing configured (for commands)
4. Azure Service Bus connection string valid
5. Queues created (run with `EnableInstallers()`)

### Messages Failing with Deserialization Error
**Check**:
1. Message contract matches on both ends
2. Same serialization settings (JSON, XML, etc.)
3. Namespace and type name match exactly

### Saga Not Correlating Messages
**Check**:
1. `ConfigureHowToFindSaga()` configured
2. Correlation property exists in message
3. Correlation property matches saga data property
4. Saga persistence configured correctly

### Performance Issues
**Check**:
1. Concurrency settings (max concurrent handlers)
2. Message processing time (optimize handlers)
3. Database query performance
4. Retry policy (too many retries?)

---

**Next**: See [Data Patterns](data-patterns.md) for CosmosDB repository implementation, partition strategies, and CQRS patterns.
