using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SharedKernel.EventSourcing.EventStore;

internal sealed class EventStoreFactory<TEventStore>(CosmosClient cosmosClient, 
    IOptionsMonitor<EventStoreOptions> namedOptionsAccessor, ILoggerFactory loggerFactory)
    : IEventStoreFactory<TEventStore> where TEventStore : class
{
    public TEventStore Create()
    {
        // TODO: Modify this if moving to separate repo
        var logger = loggerFactory.CreateLogger("EventStore"); 
        var eventStoreType = typeof(TEventStore);
        return ((TEventStore)Activator.CreateInstance(eventStoreType,
            cosmosClient, namedOptionsAccessor.Get(eventStoreType.Name), logger)!)!;
    }
}

public interface IEventStoreFactory<TEventStore>
    where TEventStore : class
{
    TEventStore Create();
}
