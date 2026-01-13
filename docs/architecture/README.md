# Distributed Event-Driven Architecture Framework

## Overview

This architecture framework enables the rapid development of distributed, event-driven applications using Domain-Driven Design (DDD) principles. The framework is designed to allow business requirements to map directly to working code through GitHub Copilot, minimizing the gap between business intent and technical implementation.

## Vision

**Enable business stakeholders to define requirements in a structured format that GitHub Copilot can transform into a fully functional, production-ready distributed application in minutes.**

## Core Principles

1. **Event-Driven Architecture**: Domains communicate exclusively through events and messages, ensuring loose coupling and scalability
2. **Domain-Driven Design**: Business domains are first-class architectural boundaries with clear ownership and responsibilities
3. **CQRS Pattern**: Separate read and write concerns for optimal performance and clarity
4. **Consistency Through Structure**: Every business domain follows an identical project structure for predictability
5. **Business-First Requirements**: Requirements documents drive architecture decisions and code generation
6. **Rapid Scaffolding**: Generate complete domain implementations from business requirements in minutes

## Architecture Components

### Platform Domain (Required)
The Platform domain is the foundation that hosts all business domains:
- **UI Shell**: Main navigation, routing, and micro-frontend composition
- **Shared Infrastructure**: SignalR handlers, common utilities, authentication
- **Common Contracts**: Cross-domain events and interfaces
- **Hosting**: API and messaging endpoints for platform-level concerns

### Business Domains (Variable)
Each business domain is an independent, self-contained module:
- **Api**: HTTP endpoints for external access
- **Domain**: Core business logic, entities, and domain events
- **Endpoint.In**: Background message processing and sagas
- **Infrastructure**: Data access (CosmosDB), external integrations, NServiceBus configuration
- **Test**: Unit and integration tests
- **UI**: React components and pages (optional, for domains with UI needs)

## Document Navigation

### Getting Started
1. **[Event Storming Guide](event-storming-guide.md)** - Discover domains, events, and workflows from business requirements
2. **[Requirements Document Template](requirements-document-template.md)** - Standard format for business requirements
3. **[Initial Scaffolding Prompts](initial-scaffolding-prompts.md)** - Generate a complete application from business requirements

### Architecture Deep Dive
4. **[Domain Template Structure](domain-template-structure.md)** - Exact project structure every domain must follow
5. **[Platform Domain Responsibilities](platform-domain-responsibilities.md)** - What belongs in Platform vs. business domains
6. **[Requirements to Architecture Mapping](requirements-to-architecture-mapping.md)** - How business concepts become code

### Technical Patterns
7. **[Event-Driven Patterns](event-driven-patterns.md)** - Events, event ownership, naming conventions, cross-domain communication
8. **[NServiceBus Patterns](nservicebus-patterns.md)** - Commands, sagas, handlers, retries, error handling
9. **[Data Patterns](data-patterns.md)** - CosmosDB per domain, partition keys, repositories, CQRS
10. **[UI Architecture](ui-architecture.md)** - React micro-frontends, Module Federation, shared vs. domain UI

### Specialized Patterns
11. **[Validation Workflow Pattern](validation-workflow-pattern.md)** - Markdown-driven validation with GitHub Actions
12. **[Bulk Import Pattern](bulk-import-pattern.md)** - File upload → saga → SignalR → results workflow
13. **[SignalR Real-Time Patterns](signalr-realtime-patterns.md)** - Real-time updates, event handlers, connection management

### Examples
14. **[Worked Example: Game Application](worked-example-game-application.md)** - Complete example with User Account, Questions, and Points domains

## Quick Start

### For Business Stakeholders
1. Read the [Event Storming Guide](event-storming-guide.md) to understand how to discover domains and events
2. Use the [Requirements Document Template](requirements-document-template.md) to document your application needs
3. Follow the [Initial Scaffolding Prompts](initial-scaffolding-prompts.md) to generate your application with GitHub Copilot

### For Developers
1. Review [Domain Template Structure](domain-template-structure.md) to understand the project layout
2. Study [Event-Driven Patterns](event-driven-patterns.md) and [NServiceBus Patterns](nservicebus-patterns.md)
3. Reference [Data Patterns](data-patterns.md) and [UI Architecture](ui-architecture.md) as needed
4. Use [Worked Example: Game Application](worked-example-game-application.md) as a reference implementation

### For Architects
1. Review all Technical Patterns documents to understand design decisions
2. Study [Platform Domain Responsibilities](platform-domain-responsibilities.md) for cross-cutting concerns
3. Use [Requirements to Architecture Mapping](requirements-to-architecture-mapping.md) to validate domain boundaries
4. Reference [Event-Driven Patterns](event-driven-patterns.md) for event ownership strategies

## Technology Stack

### Backend
- **.NET 8**: Core framework
- **Azure Functions (Isolated Worker)** OR **Docker Containers**: Compute hosting (interchangeable)
- **NServiceBus**: Messaging framework for commands, events, and sagas
- **Azure CosmosDB**: NoSQL database (one per domain)
- **Azure Service Bus**: Message broker
- **xUnit**: Testing framework

### Frontend
- **React 18**: UI framework
- **TypeScript**: Type safety
- **Material-UI (MUI)**: Component library
- **Module Federation**: Micro-frontend architecture
- **React Router**: Client-side routing
- **React Hook Form**: Form validation

### Infrastructure
- **Azure SignalR**: Real-time communication
- **Azurite**: Local Azure Storage emulator
- **Docker**: Containerization (optional)

## Key Architectural Decisions

### Why Event-Driven?
- **Loose Coupling**: Domains don't depend on each other's APIs
- **Scalability**: Each domain scales independently
- **Resilience**: Message retries and error handling built-in
- **Audit Trail**: Events provide complete history

### Why One Database Per Domain?
- **Autonomy**: Each domain owns its data schema
- **Independent Deployment**: Schema changes don't affect other domains
- **Clear Boundaries**: No shared database dependencies
- **Performance**: Partition keys optimized per domain

### Why CQRS?
- **Optimized Reads**: Query models tailored for UI needs
- **Optimized Writes**: Command models enforce business rules
- **Scalability**: Read and write paths scale independently
- **Clarity**: Separation of concerns between queries and commands

### Why Micro-Frontends?
- **Independent Deployment**: UI modules deploy separately
- **Team Autonomy**: Domain teams own their UI
- **Technology Flexibility**: Different React versions/libraries possible
- **Composability**: Platform shell composes domain UIs

## Success Metrics

A well-implemented application using this framework should achieve:
- **Rapid Development**: New domain created in < 30 minutes
- **Clear Boundaries**: No direct dependencies between domains
- **Complete Tests**: > 80% code coverage with meaningful tests
- **Self-Documenting**: Requirements documents explain all behavior
- **Consistent Structure**: All domains follow identical patterns
- **Production Ready**: Includes logging, error handling, monitoring hooks

## Contributing to This Framework

This framework is designed to evolve. When adding new patterns:
1. Document the business problem it solves
2. Provide code examples
3. Include mermaid diagrams for complex flows
4. Add to the appropriate section in this README
5. Update the [Worked Example](worked-example-game-application.md) if applicable

## Support

For questions or clarifications:
- Review the [Worked Example](worked-example-game-application.md) for a complete implementation
- Check existing domains (Beneficiary, Medical) for reference patterns
- Consult the specific pattern document for deep technical details

---

**Next Steps**: Start with the [Event Storming Guide](event-storming-guide.md) to discover your domains, then use the [Requirements Document Template](requirements-document-template.md) to document your application needs.
