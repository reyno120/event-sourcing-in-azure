using System.Text.Json;

namespace SharedKernel;

public class EventStream
{
    public DateTimeOffset TimeStamp = DateTimeOffset.UtcNow;
    public Guid EventId = Guid.NewGuid();
    public Guid StreamId { get; init; }
    public Type EventType { get; init; }
    public int Version { get; init; }
    public string Payload { get; init; }
}

public static class EventStreamExtensions 
{
    public static object Deserialize(this EventStream stream)
    {
        return JsonSerializer.Deserialize(stream.Payload, stream.EventType)!;
    } 
}