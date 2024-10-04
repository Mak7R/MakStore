namespace MakStore.SharedComponents.Exceptions;

public class AlreadyExistsException : Exception
{
    public AlreadyExistsException() : base("A resource with the same identifier already exists.")
    {
    }

    public AlreadyExistsException(string message) : base(message)
    {
    }

    public AlreadyExistsException(string message, Exception inner) : base(message, inner)
    {
    }
}