using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKernel;

public class EventStream(DateTimeOffset timeStamp, Guid streamId, Type eventType, int version, string payload)
{
    public Guid Id { get; private set; } =  Guid.NewGuid(); 
    
    public DateTimeOffset TimeStamp { get; init; } = timeStamp;
    
    public Guid StreamId { get; init; } = streamId; // AggregateId & Our Partition Key
    
    [JsonConverter(typeof(TypeJsonConverter))]
    public Type EventType { get; init; } = eventType;
    
    public int Version { get; init; } = version;
    
    public string Payload { get; init; } = payload;
}

public static class EventStreamExtensions 
{
    public static object Deserialize(this EventStream stream)
    {
        return JsonSerializer.Deserialize(stream.Payload, stream.EventType)!;
    } 
}


// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-8-0
public class TypeJsonConverter : JsonConverter<Type>
{
    public override Type Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Type.GetType(reader.GetString()!)!;

    public override void Write(
        Utf8JsonWriter writer,
        Type type,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(type.AssemblyQualifiedName);
}