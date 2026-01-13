# Distributed Architecture Concepts

Provides educational links related to distributed architecture concepts

[[_TOC_]]

Distributed architecture is all about recognizing the [fallacies of distributed computing](https://en.wikipedia.org/wiki/Fallacies_of_distributed_computing) and addressing them head on from the start so that we don't develop systems that are fragile and hard to change. These links will get you started but there is a whole world out there.

[Avoiding a Failed architecture and EDA Concepts](https://www.infoq.com/presentations/SOA-Business-Autonomous-Components/) This presentation gives a decent overview of what we are trying to achieve and explains event drive architecture {EDA}.

[Semantics around commands and events](https://docs.particular.net/nservicebus/messaging/messages-events-commands)

[Aggregates across domains](https://particular.net/webinars/all-our-aggregates-are-wrong)

[Exactly one processing is easy](https://www.youtube.com/watch?v=fJ3fiBoy7Ys)

[Event Sourcing and why you shouldn’t try it](https://youtu.be/-iuMjjKQnhg)

[Differences between Events and Commands](https://docs.particular.net/nservicebus/messaging/messages-events-commands) These subtle differences make a bid difference

[Getting Started with NServiceBus](https://docs.particular.net/tutorials/quickstart/) This is the best way to learn NServiceBus

[Event Storming](https://www.capitalone.com/tech/software-engineering/event-storming-for-microservice-architecture/) Process to determine Domain Boundaries, Events, Commands, and Aggregates

[How to avoid a failed Distributed system and Event Driven Architecture](https://www.infoq.com/presentations/SOA-Business-Autonomous-Components/) A good explanation of event driven architecture.  

## Patterns

[Publisher-Subscriber](https://docs.microsoft.com/en-us/azure/architecture/patterns/publisher-subscriber) Enable an application to announce events to multiple interested consumers asynchronously, without coupling the senders to the receivers.

[Event Sourcing](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing) Instead of storing just the current state of the data in a domain, use an append-only store to record the full series of actions taken on that data.

[Saga](https://docs.microsoft.com/en-us/azure/architecture/reference-architectures/saga/saga) The Saga design pattern is a way to manage data consistency across microservices in distributed transaction scenarios.
