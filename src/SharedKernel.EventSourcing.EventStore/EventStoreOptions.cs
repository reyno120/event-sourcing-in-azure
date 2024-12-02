namespace SharedKernel.EventSourcing.EventStore;

public class EventStoreOptions
{
    public const string EventStore = "EventStore";
    
    public string? DatabaseName { get; init; } 
    public string? ContainerName { get; init; }
}