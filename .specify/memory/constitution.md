<!--
Sync Impact Report - Version 1.0.0 (2026-01-03)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Version Change: Initial → 1.0.0

Constitution Status:
  ✅ Created - Initial constitution derived from memory files
  ✅ 10 principles defined from domain patterns and standards
  ✅ Governance model established
  ✅ Compliance framework defined

Principles Defined:
  1. Bounded Context Isolation (NEW) - Domain boundaries, database per domain
  2. Message-Based Integration (NEW) - Event-driven architecture primary
  3. Eventual Consistency (NEW) - Async operations, no distributed transactions
  4. Test Coverage Requirements (NEW) - Domain 90%+, application 80%+
  5. API Async-First (NEW) - 202 Accepted pattern for commands
  6. Clean Architecture Layers (NEW) - Dependency rules, separation of concerns
  7. Idempotent Operations (NEW) - Retry-safe handlers, duplicate handling
  8. Repository Pattern (NEW) - Domain defines, infrastructure implements
  9. Naming Consistency (NEW) - Commands (imperative), events (past tense)
  10. Explicit Validation Layers (NEW) - API format validation, domain business rules

Templates Requiring Updates:
  ✅ .specify/memory/*.md - Source files used to derive principles
  ⚠ .specify/templates/plan-template.md - Verify alignment with principles
  ⚠ .specify/templates/spec-template.md - Ensure requirements align
  ⚠ .specify/templates/tasks-template.md - Task types match principles
  ⚠ .github/prompts/*.md - Update command prompts if needed

Follow-up Actions:
  □ Review template files for consistency with constitution
  □ Update command prompts to reference constitution principles
  □ Establish periodic constitution review cadence
  □ Train team on constitutional principles

Next Amendment: TBD (on principle violation or new pattern adoption)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
-->

# EventManagement Bounded Context Constitution

## Core Principles

These principles are **non-negotiable architectural rules** derived from the established patterns and standards documented in the `.specify/memory/` directory. All code, designs, and decisions MUST align with these principles.

---

### I. Bounded Context Isolation

**The Rule**: Each bounded context owns its data, enforces its own boundaries, and integrates only through defined contracts.

**Requirements**:
- One Cosmos DB database per bounded context (no shared databases)
- Domain model is internal to the context - never exposed directly
- All dependencies point inward (Domain ← Application ← Infrastructure)
- Cross-context access ONLY through published messages or APIs
- No direct repository or database calls across contexts

**Rationale**: Bounded contexts enable independent evolution, deployment, and scaling. Breaking this isolation creates tight coupling, deployment dependencies, and data integrity risks.

**Verification**: No database connection strings reference other contexts; no cross-context entity references.

---

### II. Message-Based Integration (Primary)

**The Rule**: Integration between bounded contexts is **primarily message-based** using NServiceBus and Azure Service Bus.

**Requirements**:
- Commands (imperative, unicast) for directed actions
- Events (past-tense, broadcast) for notifications
- Use `context.Publish()` for events, `context.Send()` for commands
- Correlation IDs MUST track operations across contexts
- Prefer events over commands for cross-context communication
- Synchronous HTTP calls between contexts are prohibited except for composition layers

**Rationale**: Message-based integration maintains loose coupling, enables independent scaling, provides natural retry/error handling, and supports choreography patterns.

**Verification**: No HTTP client calls between bounded contexts; all integration through Service Bus.

---

### III. Eventual Consistency

**The Rule**: Cross-context operations are eventually consistent. Distributed transactions are prohibited.

**Requirements**:
- Accept eventual consistency as the default model
- Use saga pattern for multi-step workflows requiring coordination
- Design UIs to handle "processing" states
- Implement idempotent handlers for duplicate message safety
- No `TransactionScope` across bounded contexts
- Compensating transactions via sagas when rollback needed

**Rationale**: Distributed transactions don't scale, create tight coupling, and are fragile. Eventual consistency with proper saga compensation is more reliable and scalable.

**Verification**: No TransactionScope usage; all multi-step processes use sagas.

---

### IV. Test Coverage Requirements (NON-NEGOTIABLE)

**The Rule**: Code MUST meet minimum test coverage thresholds before merging.

**Coverage Targets**:
- **Domain Layer**: 90%+ coverage (entities, aggregates, value objects)
- **Application Layer**: 80%+ coverage (services, handlers, commands)
- **Infrastructure Layer**: Integration tests for repositories and external services
- **API Layer**: Integration tests for endpoints

**Test Standards**:
- xUnit framework for all tests
- AAA pattern (Arrange-Act-Assert)
- One assertion focus per test
- Test naming: `MethodName_Scenario_ExpectedBehavior`
- Prefer fakes over mocks for repositories
- No tests against implementation details

**Rationale**: High test coverage in domain and application layers ensures business logic correctness, enables confident refactoring, and catches regressions early.

**Verification**: Code coverage reports in CI; PR gates block merge below thresholds.

---

### V. API Async-First (202 Accepted Pattern)

**The Rule**: API endpoints that trigger commands MUST return `202 Accepted` and process asynchronously.

**Requirements**:
- POST/PUT/DELETE commands return 202 Accepted immediately
- API validates request format only (not business rules)
- Command sent to message bus via `IMessageSession.Send()`
- Response includes correlation/tracking identifier
- Business validation happens in message handlers
- Only simple GET queries may return 200 OK synchronously

**Rationale**: Async-first prevents timeouts, scales better under load, decouples API from processing time, and enables proper error handling through messaging.

**Verification**: All command endpoints return 202; no business logic in controllers.

---

### VI. Architecture Layers

**The Rule**: Strict layering with inward-only dependencies: Domain ← Application ← Infrastructure.

**Layer Responsibilities**:
- **Domain**: Entities, value objects, domain events, repository interfaces (NO dependencies except primitives)
- **Application**: Services, DTOs, command/query handlers (depends on Domain only)
- **Infrastructure**: Repository implementations, external service clients, data access (depends on Domain and Application)
- **API**: Controllers, request/response models (depends on Application)
- **Message**: NServiceBus handlers (depends on Application)

**Prohibited**:
- Domain depending on Infrastructure
- Domain depending on Application
- Application depending on Infrastructure
- Entity Framework or data annotations in Domain

**Rationale**: Clean architecture enables testability, maintainability, and independent evolution of layers. Dependencies flow toward domain model (stable) from infrastructure (volatile).

**Verification**: Project references enforce layer dependencies; no Infrastructure references in Domain.

---

### VII. Idempotent Operations

**The Rule**: All message handlers MUST be idempotent (safe to retry/duplicate).

**Requirements**:
- Check for existing state before creating entities
- Use unique identifiers from commands, not auto-generated
- Support at-least-once delivery semantics
- Handle duplicate messages gracefully (no errors)
- Use optimistic concurrency (ETags) for updates
- Log duplicate detection but don't fail

**Implementation Pattern**:
```csharp
public async Task Handle(CreateEventCommand message, IMessageHandlerContext context)
{
    var existing = await _repository.GetByIdAsync(message.EventId);
    if (existing != null)
    {
        _logger.LogInformation("Event {EventId} already exists, skipping", message.EventId);
        return; // Idempotent - safe to ignore
    }
    
    // Create new event...
}
```

**Rationale**: Message systems guarantee at-least-once delivery. Non-idempotent handlers create duplicate data and errors on retry.

**Verification**: All handlers check for existing state; integration tests verify retry safety.

---

### VIII. Repository Pattern

**The Rule**: All data access goes through repository interfaces defined in Domain, implemented in Infrastructure.

**Requirements**:
- Domain defines `IEntityRepository` interfaces
- Infrastructure provides `EntityRepository` implementations
- Repositories work with domain entities, not DTOs or documents
- Mapping between entities and storage documents happens in Infrastructure
- Application layer depends on repository interfaces only
- Repositories return domain entities or collections

**Prohibited**:
- Direct `CosmosClient` usage outside repositories
- Entity Framework `DbContext` references in Domain or Application
- Data access code in handlers or services

**Rationale**: Repository pattern enables testability (mock repositories), maintainability (swap implementations), and clean separation between domain and persistence.

**Verification**: No data access code outside Infrastructure; handlers use interfaces only.

---

### IX. Naming Consistency

**The Rule**: Naming follows strict conventions that communicate intent.

**Message Naming**:
- **Commands**: `Verb` + `Noun` + `Command` (e.g., `CreateEventCommand`, `CloseEventCommand`)
- **Events**: `Noun` + `VerbPastTense` + `Event` (e.g., `EventCreatedEvent`, `EventClosedEvent`)

**Code Naming**:
- **Entities**: Singular noun, PascalCase (e.g., `Event`, `Ticket`)
- **Handlers**: `MessageName` + `Handler` (e.g., `CreateEventCommandHandler`)
- **Repositories**: `IEntityRepository`, `EntityRepository` (e.g., `IEventRepository`)
- **Services**: `EntityService` or `FunctionalService` (e.g., `EventService`)
- **Controllers**: `EntitiesController` plural (e.g., `EventsController`)

**Rationale**: Consistent naming improves readability, communicates intent, and enables code generation/scaffolding. Commands sound like actions, events like history.

**Verification**: Code reviews enforce naming; linting rules where possible.

---

### X. Explicit Validation Layers

**The Rule**: Validation happens at two distinct layers with different responsibilities.

**API Layer Validation** (Format/Structure):
- Data Annotations on DTOs (`[Required]`, `[StringLength]`, etc.)
- ModelState validation before sending commands
- Returns 400 Bad Request for format violations
- No business rule validation

**Domain Layer Validation** (Business Rules):
- Entity factory methods enforce invariants
- Business rules validated in entity methods
- Throws domain exceptions for violations
- Published as domain events or failure events

**Prohibited**:
- Business logic in API controllers
- API-layer validation of business rules
- Domain entities with public setters (use methods)
- Mixing format and business validation

**Rationale**: Separation of concerns - API validates structure/format, domain validates business meaning. Keeps domain pure and API thin.

**Verification**: No business logic in controllers; domain entities use factory methods.

---

## Architectural Constraints

### Technology Stack (Mandatory)

**Runtime & Framework**:
- .NET 9 or higher
- C# 13 with nullable reference types enabled
- Azure Cosmos DB for primary data storage
- Azure Service Bus for messaging

**Messaging Framework**:
- NServiceBus 9.x or higher
- Azure Service Bus transport
- Outbox pattern for transactional messaging

**Testing Framework**:
- xUnit for all unit and integration tests
- Fakes/mocks for dependencies (prefer fakes)

**Hosting**:
- Docker containers (Linux-based)
- Azure Container Apps for deployment

**Rationale**: Standardized stack ensures consistency, leverages team expertise, enables code reuse, and simplifies operations.

### Prohibited Technologies

**The following are explicitly prohibited**:
- ❌ Entity Framework Core (use repository pattern with Cosmos SDK)
- ❌ Distributed transactions (`TransactionScope` across services)
- ❌ Synchronous HTTP calls between bounded contexts
- ❌ Shared databases between bounded contexts
- ❌ RabbitMQ, Kafka, or other messaging systems (use Azure Service Bus)

**Rationale**: These constraints prevent anti-patterns that conflict with core principles.

---

## Development Workflow

### Feature Development Process

1. **Design Phase**:
   - Document feature in `.specify/specs/` if substantial
   - Identify affected principles from this constitution
   - Design messages (commands/events) first
   - Review with team if cross-cutting

2. **Test Phase**:
   - Write failing tests before implementation (TDD encouraged)
   - Ensure tests cover domain logic thoroughly
   - Create integration tests for repositories and handlers

3. **Implementation Phase**:
   - Start with Domain layer (entities, interfaces)
   - Add Application layer (handlers, services)
   - Implement Infrastructure (repositories, data access)
   - Add API or Message endpoints last

4. **Verification Phase**:
   - Run all tests (unit + integration)
   - Verify code coverage meets thresholds
   - Review against constitutional principles
   - Ensure naming conventions followed

### Code Review Requirements

**Every PR MUST verify**:
- ✅ Principles I-X compliance (no violations)
- ✅ Test coverage meets thresholds (Domain 90%+, Application 80%+)
- ✅ Naming conventions followed
- ✅ No prohibited technologies used
- ✅ Layer dependencies correct (no upward dependencies)
- ✅ All message handlers are idempotent

**Reviewers MUST reject PRs that**:
- Violate any core principle
- Fall below test coverage thresholds
- Introduce tight coupling or anti-patterns
- Use prohibited technologies

---

## Governance

### Constitutional Authority

This constitution **supersedes all other practices, guidelines, or preferences** in this bounded context. When conflicts arise between this document and other guidance:

1. This constitution takes precedence
2. Update conflicting guidance to align
3. If principle is wrong, amend constitution (see below)

### Amendment Process

**When to Amend**:
- Core principle proves unworkable in practice
- New architectural pattern adopted (requires principle)
- Technology constraint changes
- Principle conflicts discovered

**Amendment Procedure**:
1. **Propose Amendment**: Document issue, proposed change, rationale, impact
2. **Team Review**: Discuss with bounded context team and architect
3. **Impact Analysis**: Identify affected code, templates, and documentation
4. **Approval**: Requires architect sign-off
5. **Version Bump**: Increment version according to semantic versioning:
   - **MAJOR**: Backward-incompatible principle change (e.g., removing principle)
   - **MINOR**: New principle added or existing principle expanded
   - **PATCH**: Clarification, typo fix, non-semantic change
6. **Migration Plan**: Document how existing code will be updated
7. **Update Documents**: Sync templates, specs, and dependent files
8. **Announce**: Communicate changes to team with effective date

### Versioning Rules

**Current Version**: 1.0.0

**Semantic Versioning**:
- `MAJOR.MINOR.PATCH` format
- MAJOR: Breaking changes to principles
- MINOR: New principles or substantial additions
- PATCH: Clarifications and corrections

**Version History**:
- 1.0.0 (2026-01-03): Initial constitution with 10 core principles

### Compliance Review

**Periodic Review**:
- Quarterly review of constitution adherence
- Architecture review of recent PRs for violations
- Update training materials if patterns drift

**Violation Handling**:
- Document violation and impact
- Determine if code needs refactoring or principle needs amendment
- Create remediation plan if needed
- Update constitution or code to align

### Related Documents

**Constitutional Guidance** (`.specify/memory/`):
- [domain-overview.md](domain-overview.md) - Bounded context scope and responsibilities
- [messaging-patterns.md](messaging-patterns.md) - Commands, events, handlers (Principles II, VII, IX)
- [data-patterns.md](data-patterns.md) - Repository pattern, Cosmos DB (Principles I, VIII)
- [cross-domain-integration.md](cross-domain-integration.md) - Integration patterns (Principles II, III)
- [testing-standards.md](testing-standards.md) - Test requirements (Principle IV)
- [api-conventions.md](api-conventions.md) - API patterns (Principle V, X)
- [naming-conventions.md](naming-conventions.md) - Naming standards (Principle IX)
- [distributed-systems-fallacies.md](distributed-systems-fallacies.md) - Mitigation strategies (Principles II, III, VII)

**Operational Guidance**:
- `.specify/templates/spec-template.md` - Feature specification template
- `.specify/templates/plan-template.md` - Implementation planning template
- `.specify/templates/tasks-template.md` - Task breakdown template

---

**Version**: 1.0.0 | **Ratified**: 2026-01-03 | **Last Amended**: 2026-01-03
