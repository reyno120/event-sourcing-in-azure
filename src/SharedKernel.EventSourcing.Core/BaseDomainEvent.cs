using MediatR;

namespace SharedKernel.EventSourcing.Core;

public abstract record BaseDomainEvent : INotification
{
    public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow; 
}