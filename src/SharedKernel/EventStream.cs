using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKernel;

public class EventStream(DateTimeOffset timeStamp, Guid streamId, Type eventType, int version, string payload)
{
    public Guid Id { get; private set; } =  Guid.NewGuid(); 
    
    public DateTimeOffset TimeStamp { get; init; } = timeStamp;
    
    [PartitionKey("/streamId")]
    public Guid StreamId { get; init; } = streamId; // AggregateId & Our Partition Key
    
    [JsonConverter(typeof(TypeJsonConverter))]
    public Type EventType { get; init; } = eventType;
    
    [UniqueKey("/version")]
    public int Version { get; init; } = version;    // Unique Key
    
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


[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
public class PartitionKey(string path) : Attribute
{
    public string Path { get; } = path;
}

[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
public class UniqueKey(string path) : Attribute
{
    public string Path { get; } = path;
}