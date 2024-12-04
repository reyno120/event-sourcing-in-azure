using MediatR;

namespace SharedKernel;

public abstract record BaseDomainEvent : INotification
{
    public DateTimeOffset DateTimeOccurredUtc { get; protected set; } = DateTimeOffset.UtcNow; 
}