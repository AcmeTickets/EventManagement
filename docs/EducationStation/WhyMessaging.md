
[[_TOC_]]

# When to use Messaging
This article intends to resolve some of the confusion around when to use messaging and when to use HTTP Service Calls.  This debate is like a political debate in that it cannot be won. Rather you'll have to pick which style give you the confidence you are seeking in your architecture. Furthermore either Messaging or HTTP based integrations done incorrectly can led to a painful architecture.

One aspect that messaging surfaces immediately is how to deal with things that aren't real time and failure. These concepts can be swept under the rug until the code is moved to production, but with messaging they are front and center for technologist and business folks to discuss at the onset of the development.

# When to Use Sync HTTP API Calls
There is nothing wrong with a Sync HTTP API call in your architecture. This make sense for most read operations where latency is a concern as it will be faster since the call is not stored and there is no middle man. Also read operations are idempotent and therefore the client can retried by the client without side affects.

From UI to Business layer due to security concerns.

When making calls to legacy system that only offer API calls.

## Decoupling the application
The primary reason people prefer messaging is it decouples applications. When one application publishes a messages to another, the publisher doesn't care where the other application is, what technology it is, or if it is even running. 

## Scalability
As stated earlier RPC is faster than Messaging. HTTP makes a direct socket connection to the server, the server executes code, and then returns the response on the already open socket. With Messaging there is a middle man that takes time to process. When I say time, I'm talking about 50-100ms. Hardly noticeable to an Human.

With HTTP the workload is unknown and can spike, but the server resources and database resources are static. You can auto scale the servers, but your stuck with your database. This means if you get a spike of traffic, your web servers and db might slow down. this causes more threads to take longer and that locks up memory. Then the garbage collector has to clean this up and that slows down the process even more. 

With messaging you are predetermining the throughput and storing messages if that throughput gets exceeded. This provides a predictable performance because extra load is stored on disk in a broker instead of as threads and memory on the web server. 

Another use case is when you have a pool of resources that can work on a given item, and a list of work which needs to be distributed in a scalable fashion. 

Lots of options for performance tuning and scaling. For example is a single handler is using a lot of resources it can easily be moved to a different machine purpose built for that workload.

## Reliability
When you need to commit a transaction you'll want to use messaging. If I have a process that needs to call a service and change data then I'll get better reliability if I break that flow into 2 isolated processes connected by messaging. That way is a single part of the transaction fails I can retry it at that point and if that should fail I can compensate for that item only.

Messaging is a big component in reliability as it levels load to servers as discussed by microsoft. 
[Reliability Patterns](https://docs.microsoft.com/en-us/azure/architecture/framework/resiliency/reliability-patterns)

## Messaging patterns to align with business
Messaging makes picking the right tool or pattern for the job. While you could implement these using a synchronous HTTP API it would be much more difficult. 
* Send/Receive
* Publish / Subscribe
* Fire and Forget
* Scatter / Gather
* Saga

One component of the business is the passage of time which many system build batch processing to accomplish. THey have systems that query the database for events then take actions on those events.  With Messaging these events can persist as messages waiting to be processed at the appropriate time.

## Extendability 
Once you've incorporated messaging and decoupled parts of your application you can easily extend your app by adding new pieces that you can guarantee will not affect the other parts. Its a very good way of continually adding to a system over time.

# What is NServiceBus?
NServiceBus (NSB) a powerful, extensible framework that will help you to leverage the principles of Service-oriented architecture (SOA) to create distributed systems that are more reliable, more extensible, more scalable, and easier to update.  

NServiceBus will help  to make multiple transactional updates utilizing the principle of eventual consistency so that you do not encounter deadlocks. It will ensure that valuable customer data is not lost in the deep dark depths of a multi-megabyte log file.

# Messaging and DDD rules-of-thumb

* HTTP calls should only be used within a Bounded Context {BC}  or when integrating with a third-party HTTP API.
* Events (and therefore messaging) should be used between BCs
Calls from the browser component to an HTTP API: HTTP
* Semantics can dictate the preferred type of call:
    * Queries: HTTP
    * Commands (fire-and-forget): messaging
    * Events: messaging

A messaging AC should be stood up for integrating with each third-party HTTP API. This gateway should implement request/response semantics over messaging.

Some of these heuristics may seem counter-intuitive, but they seem to be the consensus of the most experienced expert practitioners of DDD/Event-Driven SOA and should be taken seriously.

## Why?
### Behavioral decoupling ###
Each BC is not responsible for knowing what should happen in another BC as a consequence of something happening within its own BC. It merely publishes an event. Other BCs may deploy a new AC at any time that subscribes to the event without requiring any changes on the publishing BC.

### Temporal decoupling ###
 BCs do not need to know or care whether another BC is available. An upstream BC may publish events that will eventually be consumed by other BCs, even if the consuming BC is unavailable due to a transient hardware, software or network fault. A downstream BC may subscribe to events and hold onto just enough data to avoid needing to make synchronous calls to another BC to perform a query in the future.

### Scalability ### 
 With traditional synchronous request/response, the caller must maintain a thread, consume memory and hold a database transaction open which waiting for another BC to respond. With messaging, you simply send a command or publish an event and exit, releasing all threads, memory and database transactions.

