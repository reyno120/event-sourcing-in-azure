using MediatR;

namespace EventSourcing.Core;

public abstract record BaseDomainEvent : INotification
{
    public DateTimeOffset DateTimeOccurredUtc { get; protected set; } = DateTimeOffset.UtcNow; 
}