namespace MakStore.SharedComponents.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base("The requested resource could not be found.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}