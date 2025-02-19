﻿using System.Runtime.CompilerServices;
using System.Text.Json;
using EventSourcing.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("EventSourcing.IntegrationTests")]
namespace EventSourcing;

// Logging for Library Authors
// https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-library-authors

/// <inheritdoc cref="IEventStore{T}"/>
internal partial class EventStore<T>(Container container, ILogger logger) 
    : IEventStore<T>
    where T : AggregateRoot
{
    public async Task Append(T aggregateRoot)
    {
        var domainEvents = aggregateRoot.CollectDomainEvents();
        // TODO: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        // Use action delegate so JsonSerializer.Serialize & typeof(T) isn't called when the LogLevel isn't set to Debug
        LogAppendRequest(logger, typeof(T).Name, aggregateRoot.Id, 
            domainEvents.Select(s => JsonSerializer.Serialize(s, s.GetType())));
        
        var batch = container.CreateTransactionalBatch(new PartitionKey(aggregateRoot.Id.ToString()));

        var version = aggregateRoot.Version;
        foreach (var domainEvent in domainEvents)
        {
            EventStream stream = new
            (
                streamId: aggregateRoot.Id,
                eventType: domainEvent.GetType(),
                version: ++version, 
                payload: JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            );

            batch.CreateItem(stream);
        }
        
        
        // TODO: Best way to throw/handle exception?
        using TransactionalBatchResponse response = await batch.ExecuteAsync();
        if (!response.IsSuccessStatusCode)
        {
            // TODO: CorrelationId??
            LogAppendError(logger, typeof(T).Name, aggregateRoot.Id,
                domainEvents.Select(s => JsonSerializer.Serialize(s, s.GetType()))); 
            throw new CosmosException(response.ErrorMessage, 
                response.StatusCode, 0, response.ActivityId, response.RequestCharge);
        }
        
        // TODO: Debug Log for successfully appended events - read from transactionresult stream
        
        aggregateRoot.ClearDomainEvents();
    }
    
    [LoggerMessage(Level = LogLevel.Debug, 
        Message = "Appending the following {type} events with Id: {id} \n {@events}")]
    static partial void LogAppendRequest(ILogger logger, string type, Guid id, IEnumerable<string> @events);
    
    [LoggerMessage(
        Level = LogLevel.Error, 
        Message = "Error appending the following {type} events with Id: {id} \n {@events}")]
    static partial void LogAppendError(ILogger logger, string type, Guid id, IEnumerable<string> @events);
    
    
    public async Task<T> Load(Guid id) 
    {
        LogLoadRequest(logger, typeof(T).Name, id);
        
        var events = await LoadEvents(id);
        LogLoadedEvents(logger, typeof(T).Name, id, JsonSerializer.Serialize(events));
        
        if (events.Count == 0) 
            throw new AggregateNotFoundException(id);

        return (T)Activator.CreateInstance(typeof(T), 
            events
                .Select(s => (BaseDomainEvent)s.Deserialize())
            )!;
    }
    
    public async Task<T?> TryLoad(Guid id) 
    {
        LogLoadRequest(logger, typeof(T).Name, id);
        
        var events = await LoadEvents(id);
        LogLoadedEvents(logger, typeof(T).Name, id, JsonSerializer.Serialize(events));
        
        if (events.Count == 0) 
            return null;     

        return (T)Activator.CreateInstance(typeof(T), 
            events
                .Select(s => (BaseDomainEvent)s.Deserialize())
        )!;
    }
    
    [LoggerMessage(Level = LogLevel.Debug, 
        Message = "Loading events for {type} with Id: {id}")]
    static partial void LogLoadRequest(ILogger logger, string type, Guid id);
    
    [LoggerMessage(Level = LogLevel.Debug, 
        Message = "Found {type} events with Id: {id} \n {@events}")]
    static partial void LogLoadedEvents(ILogger logger, string type, Guid id, string @events);

    private async Task<List<EventStream>> LoadEvents(Guid id)
    {
        // Query CosmosDB Event Stream
        // StreamId = AggregateId = Partition Key
        IOrderedQueryable<EventStream> queryable = container.GetItemLinqQueryable<EventStream>();

        var matches = queryable
            .Where(w => w.StreamId == id)
            .OrderBy(o => o.Version);

        using FeedIterator<EventStream> linqFeed = matches.ToFeedIterator();

        var events = new List<EventStream>();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<EventStream> response = await linqFeed.ReadNextAsync();
            events.AddRange(response);
        }

        return events;
    }
}