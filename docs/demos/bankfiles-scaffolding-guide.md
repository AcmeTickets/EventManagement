# GitHub Copilot Scaffolding Guide: Ultimate File Processing System

## Prerequisites

Before starting, ensure:
- ✅ Architecture documentation in Copilot Space: "Distributed Architecture"
- ✅ Demo requirements read: `Platform/docs/demos/ultimate-file-processing-system.md`
- ✅ GitHub Copilot extension active in VS Code
- ✅ New solution folder created: `BankFiles/`

---

## Step-by-Step Prompts

### Step 1: Create New Domain Structure

**Prompt to GitHub Copilot:**

```
I need to create a new domain called "BankFiles" for a bank file processing system.

Reference the architecture documentation in the "Distributed Architecture" Copilot Space and follow the exact patterns from:
- Platform/docs/architecture/domain-template-structure.md
- Platform/docs/demos/ultimate-file-processing-system.md

Create the complete folder structure with these 6 projects:

1. BankFiles/src/Api/ - Azure Functions (isolated worker)
2. BankFiles/src/Domain/ - Business logic, contracts, DTOs, validators
3. BankFiles/src/Endpoint.In/ - NServiceBus handlers and sagas
4. BankFiles/src/Infrastructure/ - Repositories, parsers, services
5. BankFiles/src/Test/ - xUnit tests
6. BankFiles/src/UI/ - React TypeScript application

Include:
- .csproj files with correct NuGet packages (NServiceBus 9.0, Azure Functions, CosmosDB, FluentValidation)
- Program.cs for each backend project with NServiceBus configuration
- package.json for UI with React 18, TypeScript, Material-UI, Module Federation
- webpack.config.js configured as a remote module
- local.settings.json.template files (don't include actual secrets)
- host.json for Azure Functions
- Solution.sln file

Use Beneficiary domain as the reference template.
```

**Expected Output:**
- Complete folder structure
- All project files (.csproj)
- Configuration files
- Empty folders for code (Models/, Handlers/, etc.)

**Validation:**
- Solution builds: `dotnet build BankFiles/Solution.sln`
- UI dependencies install: `cd BankFiles/src/UI && npm install`

---

### Step 2: Generate Domain Models

**Prompt to GitHub Copilot:**

```
Generate domain models for the BankFiles system in BankFiles/src/Domain/Models/

Reference Platform/docs/demos/ultimate-file-processing-system.md for entity definitions.

Create these models following patterns from Platform/docs/architecture/domain-template-structure.md:

1. BankFile.cs
   - Guid Id
   - string FileName, FileFormat Format
   - ProcessingStatus Status
   - DateTime UploadedAt, CompletedAt?
   - string UploadedBy, Source
   - Statistics: TotalPayments, ValidPayments, InvalidPayments, ProcessedPayments, FailedPayments, PendingPayments
   - string BlobStorageUrl

2. Payment.cs
   - Guid Id, FileId
   - int LineNumber
   - decimal Amount
   - string FromAccount, ToAccount, Reference
   - DateTime PaymentDate
   - PaymentStatus Status
   - string ErrorMessage
   - DateTime? ProcessedAt
   - int RetryCount

3. PaymentException.cs
   - Guid Id, PaymentId, FileId
   - string ErrorMessage, ErrorType
   - DateTime OccurredAt
   - bool IsResolved
   - string ResolvedBy
   - DateTime? ResolvedAt

4. FileFormat.cs (enum)
   - CSV, MT940, BAI2, ISO20022, Excel

5. ProcessingStatus.cs (enum)
   - Uploaded, Parsing, Validating, Processing, Completed, Failed

6. PaymentStatus.cs (enum)
   - Pending, Validating, Processing, Processed, Failed

Include XML documentation comments and proper property types.
```

**Expected Output:**
- 6 model files in Domain/Models/
- Proper C# properties with types
- Enums for status fields
- XML documentation

**Validation:**
- Solution builds
- No compilation errors

---

### Step 3: Generate Commands & Events

**Prompt to GitHub Copilot:**

```
Generate NServiceBus commands and events for BankFiles domain.

Reference:
- Platform/docs/architecture/event-driven-patterns.md (events in past tense)
- Platform/docs/architecture/nservicebus-patterns.md (command structure)
- Platform/docs/demos/ultimate-file-processing-system.md (operations)

Create in BankFiles/src/Domain/Contracts/:

Commands (Commands/ folder) - imperative naming:
1. UploadFileCommand.cs
   - Guid FileId, string FileName, byte[] FileContent, FileFormat Format, string UploadedBy

2. ParseFileCommand.cs
   - Guid FileId, byte[] FileContent, FileFormat Format

3. ValidatePaymentsCommand.cs
   - Guid FileId

4. ProcessPaymentsCommand.cs
   - Guid FileId

5. RetryPaymentCommand.cs
   - Guid PaymentId, Guid FileId

6. GenerateSampleFileCommand.cs
   - FileFormat Format, int PaymentCount

Events (Events/ folder) - past tense naming:
1. FileUploadedEvent.cs
   - Guid FileId, string FileName, FileFormat Format, DateTime UploadedAt, string UploadedBy

2. FileParsedEvent.cs
   - Guid FileId, int TotalPayments, DateTime ParsedAt

3. ValidationCompletedEvent.cs
   - Guid FileId, int ValidCount, int InvalidCount, DateTime CompletedAt

4. ProcessingCompletedEvent.cs
   - Guid FileId, int SuccessCount, int FailureCount, DateTime CompletedAt

5. PaymentFailedEvent.cs
   - Guid PaymentId, Guid FileId, string ErrorMessage, DateTime FailedAt

6. FileProcessingProgressEvent.cs
   - Guid FileId, ProcessingStatus Status, string Message, int TotalPayments, int ProcessedPayments, int FailedPayments, DateTime Timestamp

Include XML documentation for all messages.
```

**Expected Output:**
- 6 command classes
- 6 event classes
- All in proper namespaces
- XML documentation

**Validation:**
- Solution builds
- Commands in Commands/ folder
- Events in Events/ folder

---

### Step 4: Generate File Processing Saga

**Prompt to GitHub Copilot:**

```
Generate the FileProcessingSaga for orchestrating the complete file processing workflow.

Reference:
- Platform/docs/architecture/nservicebus-patterns.md (saga pattern with correlation)
- Platform/docs/demos/ultimate-file-processing-system.md (saga implementation)
- Beneficiary/src/Endpoint.In/Sagas/BulkBeneficiaryUploadSaga.cs (as example)

Create in BankFiles/src/Endpoint.In/Sagas/:

FileProcessingSaga.cs:
- Inherit from Saga<FileProcessingSagaData>
- IAmStartedByMessages<FileUploadedEvent>
- IHandleMessages<FileParsedEvent>
- IHandleMessages<ValidationCompletedEvent>
- IHandleMessages<ProcessingCompletedEvent>
- IHandleTimeouts<ProcessingTimeout>

Configure correlation on FileId

Workflow:
1. FileUploadedEvent → Send ParseFileCommand, publish progress
2. FileParsedEvent → Send ValidatePaymentsCommand, publish progress
3. ValidationCompletedEvent → Send ProcessPaymentsCommand if valid payments exist, publish progress
4. ProcessingCompletedEvent → Mark complete, publish final progress
5. ProcessingTimeout → Handle timeout after 10 minutes

FileProcessingSagaData.cs:
- Guid FileId, string FileName, FileFormat Format
- ProcessingStatus Status
- DateTime UploadedAt, CompletedAt?
- int TotalPayments, ValidPayments, InvalidPayments, ProcessedPayments, FailedPayments

Include comprehensive logging and progress event publishing after each step.
```

**Expected Output:**
- FileProcessingSaga.cs
- FileProcessingSagaData.cs
- ProcessingTimeout.cs (timeout message)
- Full saga logic with correlation

**Validation:**
- Solution builds
- Saga implements all required interfaces
- ConfigureHowToFindSaga properly set up

---

### Step 5: Generate Command Handlers

**Prompt to GitHub Copilot:**

```
Generate NServiceBus command handlers for BankFiles domain.

Reference:
- Platform/docs/architecture/nservicebus-patterns.md (handler pattern)
- Platform/docs/demos/ultimate-file-processing-system.md (handler implementations)

Create in BankFiles/src/Endpoint.In/Handlers/Commands/:

1. ParseFileCommandHandler.cs
   - Inject IEnumerable<IFileParser>, ILogger
   - Select appropriate parser based on format
   - Parse file into Payment entities
   - Save to repository
   - Publish FileParsedEvent

2. ValidatePaymentsCommandHandler.cs
   - Inject IPaymentRepository, IPaymentValidator, ILogger
   - Load payments for file
   - Validate each payment using FluentValidation
   - Mark valid/invalid
   - Save validation results
   - Publish ValidationCompletedEvent

3. ProcessPaymentsCommandHandler.cs
   - Inject IPaymentRepository, IPaymentProcessingService, ILogger
   - Load valid payments
   - Process each payment (simulate API call)
   - Handle successes and failures
   - Update payment status
   - Publish ProcessingCompletedEvent

4. RetryPaymentCommandHandler.cs
   - Inject IPaymentRepository, IPaymentProcessingService, ILogger
   - Load failed payment
   - Increment retry count
   - Attempt processing again
   - Update status
   - Publish PaymentFailedEvent if still fails

Include:
- Constructor injection
- Comprehensive error handling
- Detailed logging
- Idempotency checks where appropriate
- Event publishing
```

**Expected Output:**
- 4 handler classes
- All implement IHandleMessages<TCommand>
- Proper dependency injection
- Error handling

**Validation:**
- Solution builds
- Handlers properly inject dependencies

---

### Step 6: Generate File Parsers

**Prompt to GitHub Copilot:**

```
Generate file parser implementations for different bank file formats.

Reference Platform/docs/demos/ultimate-file-processing-system.md (parser pattern)

Create in BankFiles/src/Infrastructure/Parsers/:

1. IFileParser.cs (interface)
   - Task<ParseResult> ParseAsync(Stream fileStream, FileFormat format)
   - bool CanParse(FileFormat format)

2. ParseResult.cs
   - List<Payment> Payments
   - List<string> Errors
   - bool Success

3. CsvParser.cs
   - Implement IFileParser
   - Parse CSV format: PaymentId,Amount,FromAccount,ToAccount,Reference,Date
   - Handle header row
   - Validate each line
   - Return ParseResult

4. Mt940Parser.cs
   - Implement IFileParser
   - Parse SWIFT MT940 format (simplified)
   - Extract payment details from transaction lines
   - Return ParseResult

5. Bai2Parser.cs
   - Implement IFileParser
   - Parse BAI2 format (simplified)
   - Extract payment details
   - Return ParseResult

For CSV parser, use robust parsing (handle quotes, commas in values).
For MT940/BAI2, implement simplified versions for demo purposes.

Include error handling for malformed files.
```

**Expected Output:**
- IFileParser interface
- ParseResult class
- 3 parser implementations
- CSV parser fully functional
- MT940/BAI2 simplified but working

**Validation:**
- Solution builds
- Can create sample CSV and parse it

---

### Step 7: Generate Repositories

**Prompt to GitHub Copilot:**

```
Generate CosmosDB repositories for BankFiles domain.

Reference:
- Platform/docs/architecture/data-patterns.md (repository pattern, partition keys)
- Beneficiary/src/Infrastructure/BeneficiaryRepository.cs (as example)

Create in BankFiles/src/Infrastructure/Repositories/:

1. IBankFileRepository.cs (interface in Domain project)
   - Task<BankFile> GetByIdAsync(Guid id, string partitionKey)
   - Task<List<BankFile>> GetRecentFilesAsync(int count)
   - Task SaveAsync(BankFile file)
   - Task UpdateAsync(BankFile file)

2. BankFileRepository.cs
   - Implement IBankFileRepository
   - Container name: "BankFiles"
   - Partition key: /uploadedDate (string format YYYY-MM-DD)
   - Map between BankFile and BankFileDocument
   - Include error handling

3. IPaymentRepository.cs (interface in Domain project)
   - Task<Payment> GetByIdAsync(Guid id, Guid fileId)
   - Task<List<Payment>> GetByFileIdAsync(Guid fileId)
   - Task<List<Payment>> GetFailedPaymentsAsync()
   - Task SaveAsync(Payment payment)
   - Task SaveBatchAsync(List<Payment> payments)
   - Task UpdateAsync(Payment payment)

4. PaymentRepository.cs
   - Implement IPaymentRepository
   - Container name: "Payments"
   - Partition key: /fileId (Guid as string)
   - Map between Payment and PaymentDocument
   - Batch operations support

Include:
- Document models (BankFileDocument, PaymentDocument)
- Proper error handling for CosmosDB exceptions
- Logging
- Constructor injection of CosmosClient
```

**Expected Output:**
- 2 repository interfaces in Domain
- 2 repository implementations in Infrastructure
- 2 document model classes
- Proper partition key strategy

**Validation:**
- Solution builds
- Interfaces in Domain, implementations in Infrastructure

---

### Step 8: Generate API Endpoints

**Prompt to GitHub Copilot:**

```
Generate Azure Functions HTTP endpoints for BankFiles API.

Reference:
- Platform/docs/architecture/domain-template-structure.md (Azure Functions pattern)
- Beneficiary/src/Api/ (as example)
- Platform/docs/demos/ultimate-file-processing-system.md (API operations)

Create in BankFiles/src/Api/:

1. UploadFileFunction.cs
   - POST /api/files
   - Accept multipart/form-data
   - Read file stream
   - Send UploadFileCommand
   - Return 202 Accepted with FileId

2. GetFileStatusFunction.cs
   - GET /api/files/{fileId}
   - Query repository
   - Return file details with statistics

3. GetRecentFilesFunction.cs
   - GET /api/files
   - Query parameter: count (default 20)
   - Return list of recent files

4. GetFileDetailsFunction.cs
   - GET /api/files/{fileId}/details
   - Return file info + all payments

5. GetFailedPaymentsFunction.cs
   - GET /api/payments/exceptions
   - Return all failed payments across all files

6. RetryPaymentFunction.cs
   - POST /api/payments/{paymentId}/retry
   - Send RetryPaymentCommand
   - Return 202 Accepted

7. BulkRetryFunction.cs
   - POST /api/payments/bulk-retry
   - Body: { paymentIds: [] }
   - Send RetryPaymentCommand for each
   - Return 202 Accepted

All functions:
- Use isolated worker model
- Inject IMessageSession (NServiceBus)
- Inject repositories for queries
- Include error handling
- Return appropriate HTTP status codes
- Add CORS headers
- Include logging
```

**Expected Output:**
- 7 Azure Function classes
- Proper HTTP triggers
- NServiceBus integration
- Error handling

**Validation:**
- Solution builds
- Functions have correct routes
- Can start with: `func start --port 7080`

---

### Step 9: Generate React UI

**Prompt to GitHub Copilot:**

```
Generate React TypeScript UI for BankFiles domain.

Reference:
- Platform/docs/architecture/ui-architecture.md (React patterns, Module Federation)
- Platform/docs/demos/ultimate-file-processing-system.md (UI components)
- Beneficiary/src/UI/ (as example)
- Platform/src/UI/src/theme/theme.ts (AcmeCorp theme)

Create in BankFiles/src/UI/src/:

1. pages/Dashboard.tsx
   - Grid of FileStatusCard components
   - SignalR connection for real-time updates
   - Load recent files on mount
   - Navigate to file detail on click

2. pages/FileUpload.tsx
   - Drag-and-drop file upload (react-dropzone)
   - File format selector
   - Upload button
   - Progress indicator
   - Success/error messages

3. pages/FileDetail.tsx
   - File information header
   - Statistics cards (total, processed, failed, pending)
   - Filter toggle (all/success/failed)
   - PaymentTable component
   - SignalR updates for live progress

4. pages/ExceptionWorkflow.tsx
   - List of all failed payments
   - Checkbox selection for bulk actions
   - Bulk retry button
   - ExceptionCard components
   - Inline editing for fixing payment data

5. components/FileStatusCard.tsx
   - Material-UI Card
   - File name, format, upload date
   - Status chip with color coding
   - Progress bar for in-progress files
   - Statistics (total, processed, failed)
   - Click handler for navigation

6. components/PaymentTable.tsx
   - Material-UI Table
   - Columns: ID, Amount, From/To Account, Status, Error, Actions
   - Retry button for failed payments
   - Error tooltip
   - Sorting support

7. components/ExceptionCard.tsx
   - Card showing failed payment
   - Expandable to show full details
   - Inline edit form for fixing data
   - Retry button
   - Checkbox for bulk selection

8. hooks/useBankFiles.ts
   - Custom hook for API calls
   - loadRecentFiles()
   - loadFileDetails(fileId)
   - uploadFile(file, format)
   - State management (loading, error, data)

9. hooks/usePayments.ts
   - Custom hook for payment operations
   - loadFailedPayments()
   - retryPayment(paymentId)
   - bulkRetryPayments(paymentIds)

10. hooks/useFileProcessingSignalR.ts
    - SignalR connection management
    - Subscribe to FileProcessingProgress event
    - Connection state (connected, disconnected, reconnecting)
    - Auto-reconnect with exponential backoff

11. App.tsx
    - React Router setup
    - Routes for all pages
    - Layout with navigation
    - Theme provider

Use:
- Material-UI components
- AcmeCorp theme colors (#0072CE blue, #FF6B35 orange)
- TypeScript types for all props
- React Hook Form for forms
- Proper error boundaries
```

**Expected Output:**
- Complete React application
- 4 pages, 3 components, 3 hooks
- App.tsx with routing
- TypeScript types throughout

**Validation:**
- `npm install` succeeds
- `npm start` runs without errors
- UI loads at http://localhost:3000

---

### Step 10: Generate Validators

**Prompt to GitHub Copilot:**

```
Generate FluentValidation validators for BankFiles domain.

Reference:
- Platform/docs/architecture/validation-workflow-pattern.md (FluentValidation pattern)
- Beneficiary/src/Domain/Validators/ (as example)

Create in BankFiles/src/Domain/Validators/:

1. UploadFileValidator.cs
   - AbstractValidator<UploadFileCommand>
   - FileName: Required, max 255 chars, valid extensions
   - FileContent: Required, max 50MB
   - Format: Must be valid FileFormat enum value
   - UploadedBy: Required, valid Guid

2. PaymentValidator.cs
   - AbstractValidator<Payment>
   - Amount: Must be > 0, max 1,000,000
   - FromAccount: Required, format "ACC####" (4 digits)
   - ToAccount: Required, format "ACC####", different from FromAccount
   - Reference: Required, max 100 chars
   - PaymentDate: Cannot be in past

3. ParseFileValidator.cs
   - AbstractValidator<ParseFileCommand>
   - FileId: Required, valid Guid
   - FileContent: Required
   - Format: Required, supported format

Include:
- Custom error messages
- Severity levels (Error for blocking, Warning for non-blocking)
- Cross-field validations where appropriate
- Async validations for database checks (e.g., duplicate file names)
```

**Expected Output:**
- 3 validator classes
- FluentValidation rules
- Custom error messages

**Validation:**
- Solution builds
- Validators can be instantiated
- Rules execute correctly

---

### Step 11: Generate SignalR Handler

**Prompt to GitHub Copilot:**

```
Generate SignalR event handler for real-time progress notifications.

Reference:
- Platform/docs/architecture/signalr-patterns.md (event-driven SignalR handlers)
- Platform/src/Endpoint.In/Handlers/Events/SignalR/ (as example)

Create in BankFiles/src/Endpoint.In/Handlers/Events/:

SignalRProgressHandler.cs
- IHandleMessages<FileProcessingProgressEvent>
- Inject IHubContext<NotificationHub> (from Platform)
- On event received:
  - Send to SignalR group named "file:{fileId}"
  - Send method: "FileProcessingProgress"
  - Include all event data
- Include try/catch (SignalR failures shouldn't break message processing)
- Log errors as warnings, not errors
- Don't rethrow exceptions

Configure in Program.cs to reference Platform's NotificationHub.
```

**Expected Output:**
- SignalRProgressHandler.cs
- Proper hub context injection
- Error handling

**Validation:**
- Solution builds
- Handler references Platform hub

---

### Step 12: Generate Tests

**Prompt to GitHub Copilot:**

```
Generate comprehensive unit tests for BankFiles domain.

Reference:
- Beneficiary/src/Test/ (as example)
- xUnit patterns

Create in BankFiles/src/Test/:

1. Validators/PaymentValidatorTests.cs
   - Test valid payment passes
   - Test amount validations (negative, zero, too large)
   - Test account format validations
   - Test same account validation
   - Test reference validation
   - Test date validation
   - Use FluentValidation.TestHelper

2. Parsers/CsvParserTests.cs
   - Test valid CSV parses correctly
   - Test malformed CSV returns errors
   - Test empty file
   - Test header-only file
   - Test missing columns
   - Test invalid data types

3. Handlers/ParseFileCommandHandlerTests.cs
   - Mock IFileParser, ILogger
   - Test successful parse publishes event
   - Test unsupported format throws exception
   - Test parser errors handled correctly

4. Sagas/FileProcessingSagaTests.cs
   - Test saga starts on FileUploadedEvent
   - Test saga transitions through all states
   - Test saga completes correctly
   - Test timeout handling
   - Use NServiceBus.Testing

5. Repositories/PaymentRepositoryTests.cs
   - Mock CosmosDB Container
   - Test SaveAsync
   - Test GetByFileIdAsync
   - Test batch operations
   - Test partition key generation

Use:
- xUnit [Fact] and [Theory]
- Moq for mocking
- FluentAssertions for assertions
- Arrange-Act-Assert pattern
```

**Expected Output:**
- 5 test classes
- Multiple test methods per class
- Proper mocking
- Good code coverage

**Validation:**
- Tests build
- Tests run: `dotnet test`
- All tests pass

---

### Step 13: Generate Copilot File Generator Service

**Prompt to GitHub Copilot:**

```
Generate background service that auto-generates sample bank files for demo purposes.

Reference Platform/docs/demos/ultimate-file-processing-system.md (CopilotFileGeneratorService)

Create in BankFiles/src/Infrastructure/Services/:

CopilotFileGeneratorService.cs
- Inherit from BackgroundService
- Inject IMessageSession, ILogger
- ExecuteAsync: Run every 5 minutes
- Generate random format (CSV, MT940, or BAI2)
- Generate 10-50 random payments per file
- Inject 10% failure rate (invalid account numbers)
- Send UploadFileCommand with generated content
- Helper methods:
  - GenerateCsvFile(): Create CSV with header + random rows
  - GenerateMt940File(): Create simplified MT940 format
  - GenerateBai2File(): Create simplified BAI2 format

Register service in Program.cs of Endpoint.In project.

Include comprehensive logging of generated files.
```

**Expected Output:**
- CopilotFileGeneratorService.cs
- Background service logic
- File generation methods

**Validation:**
- Solution builds
- Service registered in DI
- Files generated when running

---

### Step 14: Configure Module Federation

**Prompt to GitHub Copilot:**

```
Configure BankFiles UI as a Module Federation remote module.

Reference:
- Platform/docs/architecture/ui-architecture.md (Module Federation setup)
- Beneficiary/src/UI/webpack.config.js (as example)

Update BankFiles/src/UI/webpack.config.js:
- Configure as remote module named "bankfiles"
- Expose "./BankFilesApp" component
- Share: react, react-dom, react-router-dom, @mui/material
- Singleton: true for shared dependencies
- Remote entry: "bankfilesRemoteEntry.js"
- Port: 3002

Create BankFilesApp.tsx:
- Export main app component with routes
- Routes:
  - / → Dashboard
  - /upload → FileUpload
  - /file/:fileId → FileDetail
  - /exceptions → ExceptionWorkflow
- Use React Router

Update Platform/src/UI/webpack.config.js:
- Add bankfiles remote: "bankfiles@http://localhost:3002/bankfilesRemoteEntry.js"

Update Platform/src/UI/src/App.tsx:
- Lazy load BankFilesApp
- Add route: /bankfiles/* → <BankFilesApp />
```

**Expected Output:**
- webpack.config.js configured
- BankFilesApp.tsx created
- Platform updated to consume remote

**Validation:**
- Both UIs build
- Platform loads BankFiles routes
- Navigation works

---

## Quick Start Commands

After all generation complete:

```bash
# Build entire solution
cd BankFiles
dotnet build Solution.sln

# Install UI dependencies
cd src/UI
npm install

# Start all services (use separate terminals)

# Terminal 1: API
cd src/Api
func start --port 7080 --cors "http://localhost:3002"

# Terminal 2: Messaging
cd src/Endpoint.In
func start --port 7081

# Terminal 3: UI
cd src/UI
npm start

# Terminal 4: Platform UI (for Module Federation shell)
cd ../../Platform/src/UI
npm start
```

---

## Verification Checklist

After completing all steps:

- [ ] Solution builds without errors
- [ ] All tests pass
- [ ] API functions start and respond
- [ ] NServiceBus endpoint starts
- [ ] UI starts and loads
- [ ] File upload works
- [ ] SignalR connection established
- [ ] Progress updates appear in real-time
- [ ] File detail view shows payments
- [ ] Exception workflow displays failures
- [ ] Retry functionality works
- [ ] Auto-generated files appear every 5 minutes
- [ ] Module Federation loads in Platform shell

---

## Common Issues & Solutions

**Build Errors:**
- Ensure all NuGet packages restored: `dotnet restore`
- Check .NET 8 SDK installed: `dotnet --version`

**Function Not Starting:**
- Check Azurite running: `azurite --silent`
- Verify local.settings.json exists (copy from template)
- Check port not in use

**UI Not Loading:**
- Clear npm cache: `npm cache clean --force`
- Delete node_modules and reinstall
- Check webpack config for syntax errors

**SignalR Not Connecting:**
- Ensure Platform API running (has NotificationHub)
- Check CORS configuration
- Verify SignalR connection string in local.settings.json

---

## Next Steps After Scaffolding

1. **Run End-to-End Test**: Upload sample CSV file, verify complete workflow
2. **Customize Parsers**: Enhance MT940/BAI2 parsers with real format details
3. **Add Analytics**: Create dashboard showing processing trends
4. **Enhance Exception Handling**: Add AI-powered suggested fixes
5. **Deploy to Azure**: Create Azure resources and deploy

---

**You now have a complete, production-ready bank file processing system built using GitHub Copilot and the AcmeCorp architecture!**
