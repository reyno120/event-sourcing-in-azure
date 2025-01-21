using EventSourcing.Core;

namespace EventSourcing;

internal sealed class EventStoreManager<T>(IEventStoreFactory<T> factory) : IEventStore<T>
    where T : AggregateRoot
{
    // TODO: await not allowed in body of lock. Look at Monitor api
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/lock-semantics
    // https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor?view=net-9.0
    
    // private volatile object? _syncObj;
    // private volatile EventStore<T>? _store;
    // private EventStore<T> Store
    // {
    //     get
    //     {
    //         if (_store is EventStore<T> store)
    //             return store;
    //         
    //         lock (_syncObj ?? Interlocked.CompareExchange(ref _syncObj, new object(), null) ?? _syncObj)
    //             return _store ??= factory.Create();
    //     }
    // }

    private readonly Task<EventStore<T>> _initialization = factory.Create();
    private EventStore<T>? _store;
    
    public async Task Append(T aggregateRoot)
    {
        _store ??= await _initialization;
        await _store.Append(aggregateRoot);
    }

    public async Task<T> Load(Guid id)
    {
        _store ??= await _initialization;
        return await _store.Load(id);
    }

    public async Task<T?> TryLoad(Guid id)
    {
        _store ??= await _initialization;
        return await _store.TryLoad(id); 
    }
}