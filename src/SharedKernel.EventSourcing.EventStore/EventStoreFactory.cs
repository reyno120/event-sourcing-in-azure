using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace SharedKernel.EventSourcing.EventStore;

internal sealed class EventStoreFactory<TEventStore>(CosmosClient cosmosClient, 
    IOptionsMonitor<EventStoreOptions> namedOptionsAccessor)
    : IEventStoreFactory<TEventStore> where TEventStore : class
{
    public TEventStore Create()
    {
        var eventStoreType = typeof(TEventStore);
        return ((TEventStore)Activator.CreateInstance(eventStoreType,
            cosmosClient, namedOptionsAccessor.Get(eventStoreType.Name))!)!;
    }
}

public interface IEventStoreFactory<TEventStore>
    where TEventStore : class
{
    TEventStore Create();
}
