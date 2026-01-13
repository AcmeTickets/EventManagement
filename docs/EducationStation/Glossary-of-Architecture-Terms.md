[[_TOC_]]

##Glossary of Architecture Terms

This glossary of  architecture terms is reference documentation. Additional documentation, training, and code examples will be provided as needed to better explain the overall architecture. The architecture described by these terms draws heavily upon well-known and accepted architectural styles such as Domain-Driven Design, Service-Oriented Architecture, Event-Driven Architecture, Port and Adapters, and others. Figure 1 provides an overview of how selected elements of the problem space and solution space models relate to each other.

![HighLevel.PNG](/.attachments/HighLevel-64a851a4-7631-4e7d-91e3-9d292fd99094.PNG)
_Figure 1. Selected architecture elements from problem space and solution space_

**Adapter** : Part of the Application Service. Adapters are similar to an Integration Service or Anti-Corruption Layer, in that they translate between different messaging mechanisms and the service&#39;s internal domain view. Integrations with external systems are different because services in our top-level bounded context are interacting with those external systems on their terms (See Integration Service for more details). Adapters handle translations for interactions entering and exiting the service.

**Anti-Corruption Layer** : See Integration Service.

**API** : An Application Service receives requests through its public API. Even though each part of the API is implemented by a service, the overall API is owned by the top-level bounded context. This is necessary to ensure that the API as a whole is consistent and that its consumers do not need to be aware of the underlying service decomposition.

While most APIs use HTTP and REST as the messaging mechanism, the API is not necessarily limited to a single messaging mechanism. For example, an API may also provide push notifications (using Web Sockets, SignalR or other technologies) along with HTTP/REST. Each business capability implemented in the Application Service&#39;s interface can be referenced by one or more types of adapters, which provide different messaging mechanisms described in the API contract.

**Application Service** : The Application Service layer has the overall ownership the business capabilities mapped to the service. This responsibility is divided into the follow two categories.

1. API: The Application Service fulfills its API responsibilities providing the adapters necessary for the messaging mechanisms specified by the part of the API the service is responsible for implementing. The adapters must also translate between the technology-specific language of their related messaging mechanisms to the technology-agnostic language inside the service. The Application Service must also expose a contract representing the business capabilities owned by the service and that all adapters and integration services utilize.
2. Coordination: The Application Service fulfills its responsibility to coordinate the completion of the service&#39;sbusiness capabilities by coordinating with

- the service&#39;sInfrastructure Layer and Domain Model (Aggregate),

- external systems and 3rd parties through the service&#39;sintegration services, and
- coordinating with other services with events and (less frequently) commands.

To help with more complex coordination and related error handling, the Application Service may utilize sagas. When needed to enforce consistency requirements, the Application Service may also initiate transactions.

**Autonomous Component** : One or more deployable units may constitute an autonomous component, if and only if those components are unable to fail independently. E.g., a UI component which synchronously calls a web API. If the API is down, then the UI component will be down as well. A counter example of this would be an API which raises an event to another backend endpoint. If the backend endpoint is down, this would not cause the API to be down (but may cause a delay in message processing SLAs of course).

**Bounded Context** : defines solution&#39;s boundary, and the subset of the domain model you will be working with. Domain-Driven Design allows for the boundaries of the bounded context in the technical model in the solution space to be out of sync with the boundaries of the domain model. However, service-oriented architecture requires that the boundaries of the bounded context be closely aligned with the part of the subdomains of the domain model it is associated with. Each Bounded Context has its own ubiquitous language.

**Business Capability** : Describes a distinct piece of business functionality in a subdomain of the domain. A business capability owns the business data and business operations required for it to fulfill the job is owns and may be comprised of other business capabilities. Business capabilities an important and common building block of both the domain model and the technical model. In the technical model, a service owns at least or one business capabilities. It can be helpful to arrange business capabilities in a hierarchy as part of the domain model documentation.

**Business Component** : Over time, a service may be divided into 2 or more business components in order to handle differences in how a service&#39;s business capabilities are provided to different groups of consumers. As with services, business components are comprised of one or more autonomous components (see Autonomous Components for more details). Business components maintain the same strict autonomy we prefer to exist between regular services, including separate databases and no data sharing.

For example, a sales business capability could require that strategic and non-strategic customers be provided with different levels of service (e.g., response times). What was originally a sales service could be divided into a non-strategic sales business component and a strategic sales business component.

**Command** : An asynchronous message used to reliably invoke an action in another specified part of the system. Commands are given names that indicate their imperative nature, such as _CreateNewQuote_ or _RenewPolicy_.

Because commands must be directed at a specific part of a bounded context, they create more coupling than events. For this reason, it is more common to utilize events when designing internal messaging within a service or some other bounded context than for interactions between services or bounded contexts.

**Domain** : The real world of a business or other organization. A subset of a domain, or the entire domain, can be described conceptually in a domain model. (See Domain Model for more details)

**Domain Event** : An element of the domain model that describes something of interest that happens in the domain. A domain event should be described in the ubiquitous language of the domain (e.g. _PolicyRenewed_) and can be technically implemented as an asynchronous messaging event.

**Domain Model** : A conceptual, non-technical representation of a real-world domain, which consists of its business capabilities and ubiquitous language. Event storming can be used to learn more about a domain. Categories of business capabilities within the domain are its subdomains. One helpful way of documenting a domain model is to arrange its business capabilities in a hierarchical structure and to provide details about the data and operations that comprise each business capability.

**Domain Model/aggregate** : The domain model, or aggregate, is a layer in the service internals technical model. Be careful not to confuse the domain model in the problem space (i.e., the conceptual, non-technical representation of the real-world domain) with the domain model described here and that is an element of the service internals technical model. The domain model is an optional software pattern available for handling complex and ever-changing business logic owned by a service. The terms domain model and aggregate are nearly synonymous, except for cases when a domain model has more than one aggregate. The aggregate is a collection of entity and value objects, with an aggregate root entity object. These object types and details about their correct use are described in tactical Domain-Driven Design books and other sources. The aggregate defines a transactional consistency boundary and should not have any dependencies on external technologies such as databases or even messaging infrastructure. Refer to Chapter 10, Aggregates in _Implementing Domain-Driven Design_ by Vaughan Vernon for additional essential details on this topic.

**Endpoint** : A logical component that communicates with other components using messages. Each endpoint has an identifying name, contains a collection of message handlers and/or sagas, and is deployed to a given environment (e.g. development, testing, production).

An endpoint instance is a physical deployment of an endpoint. Each endpoint instance processes messages from an input queue which contains messages for the endpoint to process. Each endpoint has at least one endpoint instance. Additional endpoint instances may be added to scale-out the endpoint. A collection of endpoint instances represents a single endpoint if and only if:

- The endpoint instances have the same name.
- The endpoint instances contain the same collection of handlers and/or sagas.
- The endpoint instances are deployed to the same environment.

(Source: [Endpoints and endpoint instances • NServiceBus • Particular Docs](https://docs.particular.net/nservicebus/endpoints/#:~:text=An%20endpoint%20is%20a%20logical,development%2C%20testing%2C%20production).))

**Event** : A type of asynchronous message that describes something that happened. Events are part of event-driven architecture and the publish-subscribe pattern, which allows a publisher to notify several subscribers. Publish-subscribe incurs less coupling than commands because the publisher does not have knowledge of its subscribers.

**Infrastructure Service** : A layer in the service internal technical models concerned only with technical capabilities such as persistence in databases, interactions with a file system, configuration of technologies like a service bus, process hosting, etc.

**Integration Service** : Also known in Domain-Driven Design as an Anti-corruption Layer. Integration consists of shared technical bridging or adapters and mappings of domain concepts that are part of a services set of business capabilities. The shared technical integrations are part of IT/Ops. The mapping of domain-specific terminology (or identifier mappings) belongs to the integration service layer. A service may have zero, one, or multiple integration services, where each maps that service&#39;sbusiness capabilities to external systems with different terminology and views of those business capabilities. Because integration service layers contain external data, names, and identifiers, care should be taken that these things are not intermingled with the service&#39;s internal domain view defined within its Application Service, Domain Model (Aggregates), and Infrastructure Service layer. For example, this means that any data an integration service may need to persist should not be intermingled with the rest of the data persisted by the service nor even use the service&#39;sinfrastructure service layer (e.g., repositories, etc.) to access its technical capabilities (e.g. databases, etc.). For example, and integration service may need to define its own repository that saves its data to a separate logical schema in the service&#39;s database.

**IT/Ops Service** : a collection of technical capabilities defined in the top-level bounded context and used by services to fulfill their business capabilities. The IT/Ops service also provides shared technical capabilities which allow one or more services to integrate with external systems.

For more details about the IT/Ops Service, see Udi Dahan&#39;s _Advanced Distributed Systems Development_ course and various entries in his [blog](https://udidahan.com/?blog=true).

**(Asynchronous) Message**: A unit of technical asynchronous communication that has a name, and properties. In our architecture, message names should describe its specific business intent. Avoid generic message names that require knowledge of message property values to discern its intent. For example, it would be better to define several messages with specific names that describe different business activities related to _policy renewal_ rather than a single message with a property inside the message that indicates which part of _policy renewal_ a message is handling. Messages with the same properties do not necessarily need to have the same name.

There are two types of asynchronous messages, commands (see Command) and events (see Event).

**top-level bounded context** The top-level bounded context that defines a set of technical capabilities, policies, and conventions which provide greater structure and consistency to a set of services. The concerns of the top-level bounded context complements and extends Service-Oriented architecture by assuring that services work together to deliver business capabilities to clients through its API (or other channels) in a consistent, seamless, and efficient manner. For example, the IT/Ops service is a set of shared technical capabilities belonging to the top-level bounded context and used by services for integration, and other shared technical needs. Other examples of these top-level bounded contexttechnical capabilities, policies, and conventions include:

- a _communications backplane_ (i.e., messaging technical and semantic conventions that define the overall interface services use within the top-level bounded context to cooperate with each other)
- a set of accepted technical capabilities and technologies (this is reflected in the top-level bounded context&#39;s API, and determines which external systems [using technologies not accepted for use in the top-level bounded context] the top-level bounded context must conform to through integrations rather than interacting through its API)
- an _entity set definition_ (this is where the domain will have the biggest impact)
- an _agent model_ (authorization, who can and can&#39;t do what)
- a set of expectations around various execution topics(sync. vs. async., responsiveness, failover/reliability, error reporting/handling)

For more details about these top-level bounded context concerns, see [NSBCon 2015: Platform-Oriented Architecture • Particular Software](https://particular.net/blog/nsbcon-2015-platform-oriented-architecture).

**Saga** : A messaging pattern that describes a message-driven state machine, initiated by the arrival of one or more specified message types. Sagas are used to orchestrate complex business workflows, and to perform compensating actions to return the system to a consistent state after failure.

**Service** : A logical abstraction in the technical model that owns one or more business capabilities. As logical abstractions, services are typically deployed in multiple parts (which maintain their logical separation) to different components or shared infrastructure in the physical system in which they execute.

**Subdomain** : In the domain model, a subdomain contains a set of business capabilities that form a logical group. In the technical model, a subdomain contains the services that own the business capabilities defined in the corresponding subdomain of the domain model.

**Technical Capability** : Common technical abilities that services use to fulfill their business capabilities. Technical capabilities are defined as top-level bounded context policies and conventions and implemented in both infrastructure service layers within services and the IT/Ops Service at the top-level bounded context level.

**Technical Model** : The conceptual model in the solution space that expresses the domain model abstractions (e.g., domain, subdomains, business capabilities, domain events, ubiquitous language) in technical terms (e.g., top-level bounded context, subdomains, services, autonomous components, events, commands, etc.). The technical model also adds technical capabilities that support the business capabilities. The ubiquitous language, business capabilities and the subdomains where they are defined in the domain model define the foundations of the technical model structure, boundaries, and the business domain terminology that appears in the technical model. The technical model (and indirectly, the domain model concepts that inform the technical model) directly influences the implementation of the system.

**Ubiquitous Language** : A universal language used to define business domain terminology that is used by both domain experts and software developers.

[1](#sdfootnote1anc)The Ports and Adapters architectural style views all interactions a service has with external systems or other services in a similar way: they all take place through ports and adapters. Even though integrations with other systems or 3rd party software does not conform to our published API (since we interact with those systems on their terms), we could arguably say that they are also just additional ports and adapters. However, because integrations are such an important part of our architecture and because they differ from API interactions, we prefer to maintain a clear separation between the way we handle (and describe) interactions that 1) conform to our API, and 2) those that do not conform to our API. In our architecture, therefore, adapters are only part of the Application Service and not Integration Services.