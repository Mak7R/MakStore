namespace MakStore.SharedComponents.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    
    public ValidationException(IDictionary<string, string[]> errors) : base("One or more fields contain invalid values.")
    {
        Errors = errors;
    }

    public ValidationException(IDictionary<string, string[]> errors, string message) : base(message)
    {
        Errors = errors;
    }

    public ValidationException(IDictionary<string, string[]> errors, string message, Exception inner) : base(message, inner)
    {
        Errors = errors;
    }
}