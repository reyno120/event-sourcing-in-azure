namespace EventSourcing.Core;

public class AggregateNotFoundException : Exception
{
    public AggregateNotFoundException()
    {
    }

    public AggregateNotFoundException(Guid id) 
        : base($"Aggregate with Id: {id} could not be found or had no events associated with it.")
    {
    }

    public AggregateNotFoundException(string message)
        : base(message)
    {
    }

    public AggregateNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}