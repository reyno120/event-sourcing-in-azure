namespace FancyToDo.Infrastructure.Configuration;

public class EventStoreOptions
{
    public const string EventStore = "EventStore";
    
    public string? DatabaseName { get; init; } 
    public string? ContainerName { get; init; }
}