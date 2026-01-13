# DDD Analysis – Tips and Knowledge

[[_TOC_]]

## DDD Purpose

DDD is about separating the What from the How. When you are going through Event Storming think in business terms. DDD and Event Storming is about bringing together all stakeholders including Developers, Operations, BA’s, and the Business to come to an agreement on business functionality.  

The Goal of DDD is to find Bounded Contexts (BC’s) and the behaviors between them including events, commands.  

In Object Oriented programming interfaces are used as extension points. In DDD Events are the extension points and fundamental building blocks of how interactions happens. Events are raised by Aggregate Roots and are always named in past tense. EG PaymentWasMade  

DDD is an art and takes practice to get used to it. This document are tips and smells to help you through the process.

## Event Storming Process Tidbits  

Do not name BC’s prematurely Use Icons or colors to identify BC’s  
What’s the minimum data that a BC needs to do their job.

## Validating BC’s

BC’s are the technical authority for a business capability and must own data and behavior. BC’s will include:

* UI Components
* Separate self-contained DB/Schema,  
* Services processing messages  
* WebAPI/Rest  
* Legacy components  

## Smell Tests to evaluate BC’s

* Do the BC’s share data?  
* Can one BC function in the absence of the other BC  
* Visualize the events and data to and from BC’s in a sequence diagrams  
* Each BC is an Actor  
* On Mural throw a sticky for each BC.  
* Need to identify the aggregate roots.

## Replicating and duplicating data

* Replicating data can be ok. You can do it but you need to evaluate the decision.
* Replicated Data cannot be changed in the BC that it was replicated to  
duplicating data is not Ok.  
* How chatty are BC’s with each other? We do not want super chatty or super clunky communication {lots of fields}  

Sharing data between BC’s is based on the concept of stability.  It might be safe to share if.

* The data is stable (i.e. generally immutable - like an ID is)  
* The concept is stable in the business domain (i.e. not likely to be subject to lots of future requirements changes that have to be replicated in both services)  

IDs trivially satisfy both guidelines, as the ID is allocated once per entity and immutable, and the concept of the ID is not a business concern, but a technical one, so we can enforce its long-term conceptual stability.  

For example, The booking date of a hotel reservation is both stable data (the booking date doesn’t change - if you want to book a different date, you start a new reservation) and stable conceptually - hotels have had booking dates on reservations for decades - this doesn’t appear likely to change.  
If you find that your replicated data violates these smell tests here are some alternatives to consider:  

* Re-align service boundaries - could some concepts be shifted from one service to another without breaking functionality but allowing the concept to live in one service?  
* Do you need to store the data in the second service in order to fulfil a business rule/requirement, or just as a convenience for a presentation layer concern (such as preparing a report, an email or a UI screen?) - perhaps a ‘composite UI’ approach might be of assistance, where a component from the first service participates in-process in the composition of the presented output

## Aggregate and Value Objects

Aggregate objects and Value Objects only make sense in each context  
A BC might have an aggregate that is a value type in another BC. For example, ‘Vehicle’ aggregate could be a value object in one BC and a Value object in another context. You cannot have 2 contexts that change the something like the license number.

You could have a ‘Vehicle’ aggregate in one BC and another ‘Vehicle’ aggregate in another BC, but they would have different properties. The Id’s of the vehicles would be the same in the different BC’s potentially using a GUID. If you copy an aggregate’s value to another BC those properties would become immutable. For example, Rating.Vehicle.ID and Claims.Vehicle.Id would be the same.  

If there is a coordinated command and event then may be an aggregate in the middle.  

Not all aggregates have a command and event corresponding to it. If you have a correlated command and event there is an aggregate in the middle.  

BC decisions are a conglomeration of all the above tidbits.  

Events are like interface points. Think about who would want to listen to each event. Therefore events should be discrete business terms.  

## Glossary

* Aggregate Root - Has identity, accepts commands, raises events in given context. This is a big topic that takes a chapter to document.  
* Value Type - Immutable object. Can be aggregate in different context.  
Bounded Context - A description of a boundary (typically a subsystem, or the work of a particular team) within which a particular model is defined and applicable  
* Event - An action or step in a process or workflow  
* Command - A valid action that a domain must execute. For example, ApplyPaymentToEquity.
