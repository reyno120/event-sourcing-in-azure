![image](https://github.com/user-attachments/assets/ddd3ab2a-bfab-4a66-9280-010964821cb6)
# Event Sourcing in Azure
[Event Sourcing](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing) is a pattern used to capture changes made to data over time. Rather than storing the current state of an object, you append the actions (events) taken upon that object to a read-only event store. By replaying the events in the order they were persisted, you obtain the current state of that object. When used with a [CQRS](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs) approach, you can create read models or [Materialized Views](https://learn.microsoft.com/en-us/azure/architecture/patterns/materialized-view) to highly optimize an application's reads. Azure CosmosDB and Azure Functions allow us to react events, providing a lot of extensability when implementing this pattern.

## Understanding Domain Events
Domain Events is a pattern stemming from [Domain Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html) that allow us to explicitly model and decouple side effects from our core domain logic. Through "Event Handlers", we can react to domain events to trigger additional behavior within the same domain that the event was raised in. Domain events also allow us to extend our application by simply adding more event handlers when needed, following the Open/Closed SOLID Principle.

While Event Sourcing is still achievable without using DDD and Domain Events, it is far more practical when making use of these patterns

For more information about Domain Events, check out [Domain events: Design and implementation](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)

While Event Sourcing is still achievable without using DDD and Domain Events, it is far more practical when making use of these modeling techniques
- Domain events link
  DDD link


## The Write Side
![image](https://github.com/user-attachments/assets/57187ef5-427c-40c6-85f0-88f0a7ebf146)


## The Read Side
![image](https://github.com/user-attachments/assets/38511a22-80bb-4da8-a1f3-4152c50b286e)
