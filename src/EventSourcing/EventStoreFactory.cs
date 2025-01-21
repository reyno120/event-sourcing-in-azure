using EventSourcing.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSourcing;

internal sealed class EventStoreFactory<T>(CosmosClient cosmosClient, 
    IOptionsMonitor<EventStoreOptions> namedOptionsAccessor, ILoggerFactory loggerFactory) 
    : IEventStoreFactory<T> where T : AggregateRoot
{
    public async Task<EventStore<T>> Create()
    {
        var options = namedOptionsAccessor.Get(typeof(T).Name) ?? 
                                     throw new ArgumentException($"{typeof(T).Name} event store is missing from configuration."); 
        
        var container = cosmosClient
            .GetContainer(options.DatabaseName, options.ContainerName);

        var containerProperties = await container.ReadContainerAsync();
        
        // TODO: Add attributes on EventStream? instead of hard coding
        // TODO: Custom Exceptions
        // TODO: Exceptions are not working correctly
        if (!containerProperties.Resource.PartitionKeyPath.Equals("/streamId"))
            throw new Exception("Invalid parition key");

        var uniqueKeyExists = containerProperties.Resource.UniqueKeyPolicy.UniqueKeys
            .SelectMany(s => s.Paths)
            .Contains("/version");
        if (!uniqueKeyExists)
            throw new Exception("Unique Key not present.");
        

        // TODO: Is this how we want to categorize the logging??
        var logger = loggerFactory.CreateLogger($"EventStore.{typeof(T).Name}");

        return new EventStore<T>(container, logger);
    }
    
}

internal interface IEventStoreFactory<T>
    where T : AggregateRoot
{
    Task<EventStore<T>> Create();
}

// TODO: Unit/Integration tests for the following
// Test exception on namedoptionsaccessor (eventstoreoptions)
// test exception for missing container/database
// test exception for containerproperties (pk and uk)
// test logging