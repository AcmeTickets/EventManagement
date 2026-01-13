# Platform SignalR Event Architecture

## Overview

The Platform project now uses a **dedicated SignalR event handler** pattern that separates real-time UI notifications from business logic processing. This ensures that:

1. **Business logic** (Sagas) focus purely on domain operations
2. **SignalR notifications** are handled independently by dedicated handlers
3. **Events** are processed by multiple handlers simultaneously
4. **Failure isolation** - SignalR failures don't affect business processing

## Architecture Diagram

```
API publishes event
        ↓
┌─────────────────────────────────────────┐
│           NServiceBus Event Bus         │
└─────────────────┬───────────────────────┘
                  ↓
    ┌─────────────┴─────────────┐
    ↓                           ↓
┌─────────────────┐    ┌────────────────────┐
│ Business Logic  │    │ SignalR Handler    │
│ (Saga)          │    │                    │
│                 │    │ Sends to SignalR   │
│ Processes data  │    │ Hub for UI         │
│ Manages state   │    │                    │
│ Publishes more  │    │ Independent from   │
│ events          │    │ business logic     │
└─────────────────┘    └────────────────────┘
```

## Key Components

### 1. Event Types (`Domain/Contracts/Events/`)

- **`BulkBeneficiaryUploadStarted`** - When upload processing begins
- **`BulkBeneficiaryUploadProgress`** - Progress updates during processing  
- **`BulkBeneficiaryUploadCompleted`** - When upload processing finishes

### 2. SignalR Handler (`Endpoint.In/Handlers/SignalRNotificationHandler.cs`)

```csharp
public class SignalRNotificationHandler : 
    IHandleMessages<BulkBeneficiaryUploadStarted>,
    IHandleMessages<BulkBeneficiaryUploadProgress>,
    IHandleMessages<BulkBeneficiaryUploadCompleted>
{
    // Handles events and sends to SignalR hub
    // Independent from business logic
    // Failures don't affect business processing
}
```

**Responsibilities:**
- Listen for platform events
- Transform events into SignalR messages
- Send to SignalR hub for real-time UI updates
- Handle SignalR-specific errors gracefully

### 3. Business Logic Saga (`Endpoint.In/Sagas/BulkBeneficiaryUploadSaga.cs`)

```csharp
public class BulkBeneficiaryUploadSaga : 
    IAmStartedByMessages<BulkBeneficiaryUploadStarted>
{
    // Focuses purely on business logic
    // No SignalR dependencies
    // Publishes domain events
}
```

**Responsibilities:**
- Manage upload processing workflow
- Track upload state and progress
- Coordinate business operations
- Publish domain events at key milestones

### 4. SignalR Service (`Infrastructure/Services/SignalRService.cs`)

```csharp
public interface ISignalRService
{
    Task SendUploadStartedAsync(BulkBeneficiaryUploadStarted uploadStarted);
    Task SendUploadProgressAsync(...);
    Task SendUploadCompletedAsync(...);
}
```

**Responsibilities:**
- Abstract SignalR communication
- Handle HTTP calls to SignalR functions
- Provide clean interface for event handlers

## Event Flow

### 1. Upload Started
```
API → Publishes BulkBeneficiaryUploadStarted
  ↓
Saga → Receives event, initializes state
  ↓
SignalR Handler → Receives event, sends to UI
  ↓
UI → Shows "Processing started" notification
```

### 2. Upload Progress (Future)
```
Saga → Publishes BulkBeneficiaryUploadProgress
  ↓
SignalR Handler → Receives event, sends to UI
  ↓
UI → Updates progress bar and status
```

### 3. Upload Completed (Future)
```
Saga → Publishes BulkBeneficiaryUploadCompleted
  ↓
SignalR Handler → Receives event, sends to UI
  ↓
UI → Shows completion status and results
```

## Benefits

### 1. **Separation of Concerns**
- Business logic stays focused on domain operations
- UI notifications are handled separately
- Easy to modify either without affecting the other

### 2. **Reliability**
- SignalR failures don't break business processing
- Business logic continues even if UI notifications fail
- Each handler can be tested independently

### 3. **Scalability**
- Multiple handlers can process the same event
- Easy to add new notification channels (email, SMS, etc.)
- Handlers can be scaled independently

### 4. **Maintainability**
- Clear responsibility boundaries
- Easier to debug and troubleshoot
- Simple to add new event types

## Adding New Events

### 1. Create Event Class
```csharp
// Domain/Contracts/Events/NewEvent.cs
public class NewEvent : IProvideCorrelationId
{
    public string CorrelationId { get; set; }
    // Add other properties
}
```

### 2. Add to SignalR Handler
```csharp
public class SignalRNotificationHandler : 
    IHandleMessages<BulkBeneficiaryUploadStarted>,
    IHandleMessages<NewEvent>  // Add this line
{
    public async Task Handle(NewEvent message, IMessageHandlerContext context)
    {
        // Handle the new event
    }
}
```

### 3. Add SignalR Service Method
```csharp
public interface ISignalRService
{
    Task SendNewEventAsync(NewEvent eventData);
}
```

### 4. Update SignalR Functions
Add new function in `Api/SignalRFunction.cs` to broadcast the new event type.

### 5. Update UI
Add handler in React `useSignalR` hook to receive and display the new event.

## Testing

### Manual Testing
1. Start Platform API and Endpoint.In services
2. Upload a file through the UI
3. Check logs for both Saga and SignalR handler processing
4. Verify UI receives real-time updates

### Unit Testing
Use `SignalREventTestHelper` to create sample events for testing handlers in isolation.

## Configuration

All SignalR configuration is centralized in:
- `Platform/src/Api/local.settings.json` - Azure SignalR connection
- `Platform/src/Endpoint.In/local.settings.json` - SignalR function URL

## Error Handling

- **SignalR Handler**: Catches exceptions, logs errors, continues processing
- **Business Saga**: Unaffected by SignalR failures
- **UI**: Shows connection status, handles reconnection automatically

This architecture ensures robust, scalable real-time notifications while maintaining clean separation between business logic and user interface concerns.