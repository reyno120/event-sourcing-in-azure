using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace SharedKernel.EventSourcing.EventStore;

public class EventStoreFactory<TEventStore>(CosmosClient cosmosClient, 
    IOptionsMonitor<EventStoreOptions> namedOptionsAccessor)
    : IEventStoreFactory<TEventStore> where TEventStore : class 
{
    public TEventStore Create()
    {
        var aggregateType = typeof(TEventStore).GetGenericArguments()[0];
        
        return ((TEventStore)Activator.CreateInstance(typeof(TEventStore),
            cosmosClient, namedOptionsAccessor.Get($"{aggregateType.Name}EventStore"))!)!;
        // TODO: Throw exception if accessor returns nothing?
    }
}

public interface IEventStoreFactory<TEventStore>
    where TEventStore : class 
{
    TEventStore Create();
}