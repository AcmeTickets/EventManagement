# SignalR Real-Time Notifications Setup

This document outlines the SignalR implementation for real-time notifications in the IOM Platform bulk beneficiary upload system.

## Architecture Overview

The system uses Azure SignalR Service to provide real-time communication between the backend processing and the React UI:

1. **UI** uploads file → **Platform API** processes and stores → **NServiceBus** publishes event
2. **Saga** handles event → **SignalR Service** sends HTTP request → **SignalR Functions** broadcast to clients
3. **React UI** receives real-time updates via **SignalR connection**

## Components

### Backend (.NET)

#### 1. SignalR Functions (`Platform/src/Api/SignalRFunction.cs`)
- `negotiate` - Provides connection info to clients
- `SendUploadStarted` - Broadcasts upload started messages
- `SendUploadProgress` - Broadcasts progress updates  
- `SendUploadCompleted` - Broadcasts completion status
- `JoinGroup` - Adds clients to upload-specific groups

#### 2. SignalR Service (`Platform/src/Infrastructure/Services/SignalRService.cs`)
- `ISignalRService` interface for sending messages
- HTTP client implementation to call SignalR functions
- Used by NServiceBus handlers and sagas

#### 3. Bulk Upload Saga (`Platform/src/Endpoint.In/Sagas/BulkBeneficiaryUploadSaga.cs`)
- Handles `BulkBeneficiaryUploadStarted` events
- Sends SignalR notifications when processing begins
- Can be extended for progress and completion notifications

### Frontend (React)

#### 1. SignalR Hook (`Platform/src/UI/src/hooks/useSignalR.ts`)
- `useSignalR` custom hook for connection management
- Event handlers for upload lifecycle
- Group join functionality for targeted messaging

#### 2. Bulk Import Component (`Platform/src/UI/src/pages/BeneficiaryBulkImport.tsx`)
- Integrated SignalR connection status display
- Real-time upload progress visualization
- Notification system for status updates

## Configuration

### Azure SignalR Service
1. Create Azure SignalR Service instance
2. Get connection string from Azure portal
3. Update `local.settings.json` files:

```json
{
  "AzureSignalRConnectionString": "Endpoint=https://your-signalr-service.service.signalr.net;AccessKey=your-access-key;Version=1.0;"
}
```

### Local Development
- Platform API runs on port 7071
- SignalR functions available at `http://localhost:7071/api/`
- React UI connects to negotiate endpoint

## Message Flow

### 1. Upload Started
```json
{
  "correlationId": "uuid",
  "uploadId": "uuid", 
  "totalRecordsCount": 100,
  "startedAt": "2024-10-12T10:00:00Z",
  "userId": "user123",
  "fileName": "beneficiaries.csv",
  "docId": "doc123"
}
```

### 2. Upload Progress (Future)
```json
{
  "correlationId": "uuid",
  "uploadId": "uuid",
  "processedRecords": 50,
  "totalRecords": 100, 
  "percentageComplete": 50.0,
  "currentStatus": "Processing records"
}
```

### 3. Upload Completed (Future)
```json
{
  "correlationId": "uuid",
  "uploadId": "uuid", 
  "totalRecords": 100,
  "successfulRecords": 95,
  "failedRecords": 5,
  "completedAt": "2024-10-12T10:05:00Z",
  "status": "Completed with errors",
  "errors": ["Invalid date format in row 10"]
}
```

## Testing

### 1. Install Dependencies
```bash
cd Platform/src/UI
npm install
```

### 2. Start Services
```bash
# Start Platform API (includes SignalR functions)
cd Platform/src/Api
func start --port 7071

# Start NServiceBus Endpoint
cd Platform/src/Endpoint.In  
func start --port 7072

# Start React UI
cd Platform/src/UI
npm start
```

### 3. Test Upload Flow
1. Navigate to bulk import page
2. Check SignalR connection status (should show "Connected")
3. Upload a CSV file with beneficiary data
4. Submit valid records for import
5. Observe real-time notifications as processing begins

### 4. Manual Testing
Send test messages to SignalR functions:

```bash
# Test upload started notification
curl -X POST http://localhost:7071/api/SendUploadStarted \
  -H "Content-Type: application/json" \
  -d '{
    "correlationId": "test-123",
    "uploadId": "upload-456", 
    "totalRecordsCount": 10,
    "startedAt": "2024-10-12T10:00:00Z",
    "userId": "testuser",
    "fileName": "test.csv",
    "docId": "doc789"
  }'
```

## Troubleshooting

### Common Issues

1. **SignalR connection fails**
   - Check Azure SignalR Service connection string
   - Verify negotiate endpoint is accessible
   - Check CORS configuration

2. **Messages not received**
   - Verify client joined correct group
   - Check correlation IDs match
   - Review function logs for errors

3. **Build errors in React**
   - Run `npm install` to install @microsoft/signalr
   - Check TypeScript import paths

### Logs
- Azure Functions: Check console output
- React: Check browser console and network tab
- SignalR: Enable detailed logging in connection builder

## Future Enhancements

1. **Authentication**: Add user authentication to SignalR connections
2. **Progress Tracking**: Implement detailed progress notifications
3. **Error Handling**: Enhanced error reporting and recovery
4. **Scaling**: Support for multiple concurrent uploads
5. **Persistence**: Store notification history for offline users