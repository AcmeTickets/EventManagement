#Command vs Query Side and DDD 
Originally Created: 6/7/2022

## Introduction
Up until recently, PACE has been nearly 100% focused on the design and implementation of the transactional side of the system – the domain model and how it handles commands and events in a way that enforces business behavior within a database transaction. This is often called the “Command” side. Recently teams have started implementing user stories that require building out the part of the application that requires populating the UI with data. This is known as the “Query” side.

The Command and Query side are often at direct odds with each other and can pull a single domain model in two different and opposing directions.

## The command side
The Command side wants a fully encapsulated domain model that does not expose its data through getters and setters. Wherever data goes, business logic follows. To keep the transactional domain model from becoming an Anemic Domain, we keep it encapsulated, and add methods that implement the behavior that changes the entire aggregate while enforcing business rules. These methods also create the commands and events to be sent over the Service Bus. This is all part and parcel of well-executed DDD. Furthermore, strongly defined aggregate boundaries prevent direct object references from one aggregate to another, making it difficult to denormalize data across aggregate types. Usually, a developer will have to fetch a set of aggregates, extract a list of “foreign key” IDs to another type of aggregate, fetch those with a “SELECT WHERE IN (…)”, and then write the code to map values from matching aggregates into a single object.

## The query side
On the other hand, the Query side wants all data to be fully exposed so that it can be transported to the UI. The UI also wants data denormalized so that fetching data is efficient and fast. The UI wants to make fetching all the data and only the data needed for a particular screen to be trivially easy to do, and extremely performant.

## The warning
If we try to create a model that serves both the command side and the query side, it will do a poor job of both. This is one of the weaknesses of popular architectures, and one that we need to address and provide a long-term solution for.

## Solutions
Often, part of the solution is provided by the database technology itself. We need to investigate what kinds of indexes, views or projections Cosmos DB supports, so that pre-populated persistent data is available to quickly fetch for populating a screen. This would populate so-called “viewmodel” objects that can be bound directly to the user interface. For this reason, these objects in the database are sometimes called “persistent viewmodels.” (see the “single object” at the end of the third paragraph for the alternative.)

## Conclusion
This is not an immediate need but something we should address in the near term. 
The longer we wait, the more it will cost to undo the effects of using a single model. 
