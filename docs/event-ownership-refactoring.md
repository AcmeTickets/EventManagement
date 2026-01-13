# Event Ownership Refactoring

## Problem

Initially, the `BeneficiaryCreationSuccess` and `BeneficiaryCreationFailed` events were placed in the Platform domain (`Platform.Domain.Contracts.Events`). However, these events are actually **published by the Beneficiary domain**, which violated the principle that domains should own the events they publish.

## Solution

**Moved events from Platform domain to Beneficiary domain** to follow proper domain ownership principles.

### Changes Made

#### 1. Event Relocation
- **From**: `Platform.Domain.Contracts.Events`
- **To**: `Beneficiary.Domain.Contracts.Events`

#### 2. Updated References
- **Beneficiary.Endpoint.In**: Updated using statement to reference local events
- **Platform.Endpoint.In**: Added project reference to `Beneficiary.Domain` and updated using statement

#### 3. Updated Project References

**Platform.Endpoint.In.csproj**:
```xml
<ItemGroup>
  <ProjectReference Include="..\Domain\Domain.csproj" />
  <ProjectReference Include="..\Infrastructure\Infrastructure.csproj" />
  <ProjectReference Include="..\..\..\Payments\src\Domain\Domain.csproj" />
</ItemGroup>
```

#### 4. Message Routing Configuration

**Platform/src/Infrastructure/queues.ps1**:
```powershell
# Beneficiary Event Subscriptions (events published by Beneficiary domain)
asb-transport endpoint subscribe ASBPlatformMessageWorker Beneficiary.Domain.Contracts.Events.BeneficiaryCreationSuccess
asb-transport endpoint subscribe ASBPlatformMessageWorker Beneficiary.Domain.Contracts.Events.BeneficiaryCreationFailed
```

## Architecture Benefits

### ✅ Proper Domain Ownership
- **Publishers**: Beneficiary domain owns and publishes the events
- **Consumers**: Platform domain subscribes to and handles the events
- **Clear Boundaries**: Each domain owns its published contracts

### ✅ Cross-Domain Communication
- **Pattern**: Endpoint-to-Endpoint references are acceptable
- **Avoided**: Domain-to-Domain references (which would be problematic)
- **Result**: Clean separation with proper event flow

### ✅ Event Flow
```
Beneficiary.Domain (Publisher)
    ↓ publishes events
Beneficiary.Endpoint.In
    ↓ via NServiceBus
Platform.Endpoint.In (Consumer)
    ↓ handles events
Platform.Domain (Status Updates)
```

## File Changes

### Added Files
- `Beneficiary/src/Domain/Contracts/Events/BeneficiaryCreationSuccess.cs`
- `Beneficiary/src/Domain/Contracts/Events/BeneficiaryCreationFailed.cs`

### Removed Files
- `Platform/src/Domain/Contracts/Events/BeneficiaryCreationSuccess.cs`
- `Platform/src/Domain/Contracts/Events/BeneficiaryCreationFailed.cs`

### Modified Files
- `Beneficiary/src/Endpoint.In/Handlers/CreateBeneficiaryCommandHandler.cs` - Updated namespace
- `Platform/src/Endpoint.In/Endpoint.In.csproj` - Added Beneficiary.Domain reference
- `Platform/src/Endpoint.In/Handlers/BeneficiaryCreationStatusHandler.cs` - Updated namespace
- `Platform/src/Infrastructure/queues.ps1` - Added event subscriptions

## Alternative Considered

If cross-domain references become problematic, we could create a **shared contracts project**:
- `Shared.Contracts` - Contains cross-domain event contracts
- Both domains would reference this shared project
- This would eliminate cross-domain references entirely

However, the current endpoint-to-endpoint reference pattern is acceptable and follows NServiceBus best practices for distributed systems.

## Validation

- ✅ Beneficiary solution builds successfully
- ✅ Platform solution builds successfully  
- ✅ Events are in correct namespace
- ✅ Message routing configured properly
- ✅ No circular dependencies introduced