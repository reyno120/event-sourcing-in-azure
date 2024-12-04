using System.Collections;

namespace SharedKernel.EventSourcing.Core;

public abstract class AggregateRoot : Entity
{
    public int Version { get; init; } = 0; 

    protected AggregateRoot(IEnumerable<BaseDomainEvent> events)
    {
        foreach (var @event in events)
        { 
            Version++;
            base.Mutate(@event);
        }
    }
    
    protected AggregateRoot() {}

    public new List<BaseDomainEvent> CollectDomainEvents()
    {
        return base.CollectDomainEvents()
            .OrderBy(o => o.DateTimeOccurredUtc)
            .ToList();
    }
}