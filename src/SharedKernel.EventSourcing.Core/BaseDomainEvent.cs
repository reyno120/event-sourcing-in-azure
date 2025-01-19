using MediatR;

namespace SharedKernel.EventSourcing.Core;

public abstract record BaseDomainEvent : INotification
{
    public DateTimeOffset DateTimeOccurredUtc { get; protected set; } = DateTimeOffset.UtcNow; 
}