using System.Reflection;
using SharedKernel;

namespace FancyToDo.Infrastructure.Configuration;

public class EventStoreOptions
{
    public const string EventStore = "EventStore";
    public static readonly string PartitionKeyPath = 
        (((PartitionKey)typeof(EventStream).GetCustomAttribute(typeof(PartitionKey))!)!).Path;
    public static readonly string UniqueKeyPath = 
        (((UniqueKey)typeof(EventStream).GetCustomAttribute(typeof(UniqueKey))!)!).Path;

    public string? DatabaseName { get; init; } 
    public string? ContainerName { get; init; }
}