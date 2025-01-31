namespace EventSourcing;

public class InvalidResourceException : Exception
{
    public InvalidResourceException()
    {
    }

    public InvalidResourceException(string message)
        : base(message)
    {
    }

    public InvalidResourceException(string message, Exception inner)
        : base(message, inner)
    {
    }
} 