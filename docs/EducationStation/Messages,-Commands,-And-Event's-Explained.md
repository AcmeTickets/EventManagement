[[_TOC_]]

# Introduction
## Explanation
The system will leverage [AMQP](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-amqp-overview), Azure Service bus, and [NServiceBus](https://docs.particular.net/) to build a robust system. If you need to know why we are using messaging please read about messaging in the wiki


> We are using [NServiceBus](https://docs.particular.net/) in most cases you should not be interacting with Azure Service bus directly. [NServiceBus](https://docs.particular.net/) abstracts the transport so you can focus on 

A message is the unit of communication for [NServiceBus](https://docs.particular.net/). There are two types of messages, commands and events, that capture more of the intent and help [NServiceBus](https://docs.particular.net/) enforce messaging best practices. There are semantic differences between types of messages. It's important that you understand this as you map technology and messages to the business. Failure to do so will create confusion.



| **Command**  | **Event** |
|--|--|
| Used to _make a request to perform an action._ | Used to _communicate that an action has been performed._ |
| Has one logical owner. | Has one logical owner. |
| Should be _sent_ to the logical owner. | Should be _published_ by the logical owner. |
| Cannot be published. | Cannot be sent. |
| Cannot be subscribed to or unsubscribed from. | Can be subscribed to and unsubscribed from. |

##Validation
There are checks in place to ensure best practices are followed. Violations of the above guidelines generate the following exceptions:

- "Pub/Sub is not supported for Commands. They should be sent direct to their logical owner." - this exception is being thrown when attempting to publish a Command or subscribe to/unsubscribe from a Command.
- "Events can have multiple recipient so they should be published." - this exception will occur when attempting to use 'Send()' to send an event.
- "Reply is neither supported for Commands nor Events. Commands should be sent to their logical owner using bus.Send and bus. Events should be published." - this exception is thrown when attempting to reply with a Command or an Event.
- "Cannot configure routing for type because it is not considered a message. Message types have to either implement NServiceBus.IMessage interface or match a defined message convention." - this exception is thrown when configuring destination endpoint for a non-message type.
- "Cannot configure routing for assembly because it contains no types considered as messages. Message types have to either implement NServiceBus.IMessage interface or match a defined message convention." - this exception is thrown when configuring destination endpoint for an assembly which contains no types considered messages.
- "Cannot configure routing for namespace because it contains no types considered as messages..." - this exception is thrown when configuring destination endpoint for a namespace which contains no types considered messages.
- "Cannot configure publisher for type because it is not considered a message. Message types have to either implement NServiceBus.IMessage interface or match a defined message convention." - this exception is thrown when configuring publisher for a type that is not a message.
- "Cannot configure publisher for type because it is not considered an event. Event types have to either implement NServiceBus.IEvent interface or match a defined event convention." - this exception is thrown when configuring publisher for a type that is not an event.
- "Cannot configure publisher for type because it is a command." - this exception is thrown when configuring publisher for a command.


##Designing messages
Messages represent data contracts between endpoints. They should be designed according to the following guidelines.

Messages should:

1. be simple POCO types.
1. be as small as possible.
1. satisfy the Single Responsibility Principle. Types used for other purposes (e.g. domain objects, data access objects, or UI binding objects) should not be used as messages.

##More Conventions

Events **cannot be** validated. Events are facts that are communicated. Validating events would be equivalent to approving the existence of Earth or agreeing that the sun rises in the East and sets down in the West.

Commands are **intents**. The intent **has to be** validated. And the justification is simple. An intent is captured and transformed into an event/state. If it's not validated, then what you get is a system with garbage. Unless it's accepted by the business, garbage in garbage out is not something any client would agree to.

##Naming Conventions
Commands should be imperative. _SetMailingAddress_

Events because they have happened must be named in the past tense. _MailAddressSet_

Commands and events should make sense to someone in the business. _UpdateAddress_ is not a good name

## References
[Semantics around commands and events](https://docs.particular.net/nservicebus/messaging/messages-events-commands)

##NSB and ASB
You do not need to understand the flow of messages to be a productive developer.  This is more for architecture and operations.

- EVENTS go to a TOPIC.

- COMMANDS go to a QUEUE. Your queue name will be the name of your endpoint and NSB Creates this by default. The DevOps Story is not complete.

## What about Messages
There are cases when you just want to send data to something. In this case you can send a generic message <<Create example>>
