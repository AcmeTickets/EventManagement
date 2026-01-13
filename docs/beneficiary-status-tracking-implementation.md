# Beneficiary Status Tracking Implementation

## Overview

This document describes the implementation of individual beneficiary record status tracking using GUIDs and atomic CosmosDB updates to prevent concurrency issues during bulk beneficiary processing.

## Key Features Implemented

### 1. GUID-Based Record Tracking
- **Browser Generation**: Each beneficiary record gets a unique GUID (`RecordId`) generated in the browser before submission
- **Pipeline Tracking**: The RecordId flows through the entire processing pipeline from UI → Platform → Beneficiary domain
- **Individual Status**: Each record can be tracked and updated independently

### 2. Enhanced Data Models

#### BeneficiaryRecord (Platform.Domain.Models)
```csharp
public class BeneficiaryRecord
{
    public string RecordId { get; set; } = Guid.NewGuid().ToString();
    // ... other properties
    public ProcessingResult? Result { get; set; }
}

public class ProcessingResult
{
    public string Status { get; set; } = "Pending"; // "Pending", "Success", "Failed"
    public string? BeneficiaryId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
}
```

### 3. Atomic CosmosDB Updates

#### Concurrency Solution
- **CosmosDB Patch Operations**: Uses atomic `PatchItemAsync` to update only specific record properties
- **Prevents Race Conditions**: Multiple workers can update different records in the same document simultaneously
- **Optimistic Concurrency**: Built-in ETag handling for conflict resolution

#### Implementation in CosmosRepository
```csharp
public async Task<ItemResponse<T>> PatchBeneficiaryRecordStatusAsync<T>(
    string docId, 
    string correlationId, 
    string recordId, 
    string status, 
    string? beneficiaryId = null, 
    string? errorMessage = null,
    CancellationToken ct = default)
```

### 4. Event-Driven Status Updates

#### Enhanced Events (Beneficiary.Domain.Contracts.Events)
- `BeneficiaryCreationSuccess`: Includes RecordId for specific record identification
- `BeneficiaryCreationFailed`: Includes RecordId for specific record identification

**Note**: These events are published by the Beneficiary domain and consumed by the Platform domain for status tracking.

#### Processing Flow
1. **UI**: Generates RecordId for each beneficiary record
2. **Platform**: Stores records with RecordIds in BulkBeneficiaryUpload document
3. **Processing**: Creates CreateBeneficiaryCommand with RecordId
4. **Beneficiary Domain**: Processes individual records and publishes events with RecordId
5. **Status Updates**: BeneficiaryCreationStatusHandler updates specific record status using atomic patches

## Benefits

### Concurrency Safety
- **No Document-Level Locks**: Multiple records can be processed simultaneously
- **Atomic Updates**: Only the specific record's Result object is updated
- **Race Condition Prevention**: Patch operations are atomic and use ETags

### UI Capabilities
- **Individual Record Status**: UI can display progress for each record
- **Failed Record Filtering**: Easy to show only failed records for user review
- **Retry Individual Records**: Can retry processing for specific failed records
- **Real-time Updates**: SignalR can notify UI about individual record completions

### Performance
- **Minimal Network Traffic**: Only patch operations sent, not full documents
- **Reduced Contention**: No full document replacement operations
- **Efficient Queries**: Can query for specific status types

## Implementation Details

### Browser GUID Generation
```typescript
const recordsWithIds = result.data.map(record => ({
  ...record,
  recordId: crypto.randomUUID(), // Generate GUID for each record
  result: {
    status: 'Pending',
    beneficiaryId: null,
    errorMessage: null,
    processedAt: null
  }
}));
```

### Command Enhancement
```csharp
public class CreateBeneficiaryCommand
{
    public string CorrelationId { get; set; }
    public string UploadId { get; set; }
    public string RecordId { get; set; } // GUID to track individual record
    // ... other properties
}
```

### Event Publishing
```csharp
await context.Publish(new BeneficiaryCreationSuccess
{
    CorrelationId = command.CorrelationId,
    UploadId = command.UploadId,
    RecordId = command.RecordId, // Pass RecordId for tracking
    BeneficiaryId = beneficiaryId,
    // ... other properties
});
```

### Status Handler
```csharp
public async Task Handle(BeneficiaryCreationSuccess message, IMessageHandlerContext context)
{
    await _intakeManager.UpdateBeneficiaryStatus(
        message.CorrelationId, 
        message.RecordId, 
        "Success", 
        message.BeneficiaryId);
}
```

## Testing Recommendations

### Concurrent Processing Tests
1. **Multiple Records**: Test with large batches (100+ records)
2. **Simultaneous Updates**: Verify no race conditions occur
3. **Error Scenarios**: Test mixed success/failure scenarios
4. **Network Failures**: Test resilience during patch operations

### UI Testing
1. **Real-time Updates**: Verify individual record status updates in UI
2. **Failed Record Filtering**: Test filtering and display of failed records
3. **Status Accuracy**: Ensure UI reflects actual CosmosDB state
4. **Large Batches**: Test UI performance with many records

## Future Enhancements

### Possible Improvements
1. **Batch Patch Operations**: Group multiple record updates into single request
2. **Retry Mechanism**: Implement retry logic for failed patch operations
3. **Audit Trail**: Track all status changes with timestamps
4. **Progress Aggregation**: Calculate overall batch progress from individual statuses

### UI Enhancements
1. **Progress Bar Per Record**: Visual progress for each beneficiary
2. **Error Details Modal**: Detailed error information for failed records
3. **Bulk Retry**: Allow retrying all failed records at once
4. **Export Results**: Export processing results to CSV/Excel