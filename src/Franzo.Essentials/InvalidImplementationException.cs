namespace Franzo.Essentials;

public class InvalidImplementationException : Exception
{
    public InvalidImplementationException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
