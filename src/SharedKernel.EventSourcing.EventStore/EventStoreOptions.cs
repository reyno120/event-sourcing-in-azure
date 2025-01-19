namespace SharedKernel.EventSourcing.EventStore;

public class EventStoreOptions
{
    public string? DatabaseName { get; init; } 
    public string? ContainerName { get; init; } 
}
