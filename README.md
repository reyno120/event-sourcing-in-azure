![image](https://github.com/user-attachments/assets/3b40e1ce-9712-4027-b08b-b491ab2cf987)

# Event Sourcing in Azure
[Event Sourcing](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing) is a pattern used to capture changes made to data over time. Rather than storing the current state of an object, you append the actions (events) taken upon that object to a read-only event store. By replaying the events in the order they were persisted, you obtain the current state of that object. When used with a [CQRS](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs) approach, you can create read models or [Materialized Views](https://learn.microsoft.com/en-us/azure/architecture/patterns/materialized-view) to highly optimize an application's reads. Event sourcing is useful when your application requires full visibilty. That is, when you need to know the full history of your data and how it got there. This sample demonstrates how to leverage the power of Azure CosmosDB, Azure Functions, and Domain Driven Design techniques to practically implement Event Sourcing.


## Understanding Domain Events
Domain Events is a pattern stemming from [Domain Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html) that allow us to explicitly model and decouple side effects from our core domain logic. Through "Event Handlers", we can react to domain events to trigger additional behavior within the same domain that the event was raised in. Domain events also allow us to extend our application by simply adding more event handlers when needed, following the Open/Closed SOLID Principle.

While Event Sourcing is still achievable without using DDD and Domain Events, it is far more practical when making use of these modeling techniques.

For more information about Domain Events, check out [Domain events: Design and implementation](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)

For more information about Domain Driven Design, checkout the Domain Driven Design Fundamentals sample and Pluralsight course: [DDD Fundamentals](https://github.com/ardalis/pluralsight-ddd-fundamentals)


## The Write Side
![image](https://github.com/user-attachments/assets/57187ef5-427c-40c6-85f0-88f0a7ebf146)

As a request/command comes into the application, the domain object is loaded into memory by replaying it's event stream. The application service then call's upon the domain object's methods to perform business operations and create domain events. Upon saving the domain object, all the domain events that were created are saved to the event stream. With Cosmos DB, we can hook into the change feed using an Azure Function to react to those domain events. Another domain object may need to react to an event, carrying out it's own operations and writing to it's event stream, triggering the whole process over. We may convert the domain event to an integration event and publish it to a message bus for other systems to handle (a common use case when following an event-driven, microservice architecture). For most cases though, we will have at least one handler that creates/updates a projection (read model) and saves it to a data store.


## The Read Side
![image](https://github.com/user-attachments/assets/38511a22-80bb-4da8-a1f3-4152c50b286e)

By creating a projection during the write process, the read side is simplified. For a NoSQL data model in Cosmos DB, we can optimize the model to reduce costs and increase the speed of the query.

## Advantages
- Complete history of domain object is captured
- Extensability - Additional behaviors can be added by simply adding more event handlers
- Optimized read/writes


## Disadvantages
- Increased complexity with more moving parts
- Have to be aware of Eventual Consistency
- New way of thinking/modeling software
- Steeper learning curve when bringing on new Developers


## Running the Sample App
To run the sample, make sure the following are installed. Run the AppHost and Azure Function App. Aspire will take care of the rest.
- .NET 8.0
- .NET Aspire workload (through VS Installer or CLI)
- .NET Azure development workload (through VS Installer or CLI)
- Docker Desktop
- [CosmosDB Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-develop-emulator?tabs=windows%2Ccsharp&pivots=api-nosql#install-the-emulator)



## Resources Used to Develop Sample
- [Implementing Domain-Driven Design, by Vaughn Vernon](https://www.amazon.com/Implementing-Domain-Driven-Design-Vaughn-Vernon/dp/0321834577)
- [Domain Driven Design Fundamentals](https://github.com/ardalis/pluralsight-ddd-fundamentals)
- [Event Sourcing - the what, why & how - Anita Kvamme - NDC Oslo 2024](https://www.youtube.com/watch?v=1KlQVhVYiFU)
- [Exploring CQRS and Event Sourcing](https://www.microsoft.com/en-us/download/details.aspx?id=34774)

## Other Resources Referenced
- [Event Sourcing](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)
- [CQRS](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Materialized Views](https://learn.microsoft.com/en-us/azure/architecture/patterns/materialized-view)
- [Domain Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [Domain events: Design and implementation](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)
