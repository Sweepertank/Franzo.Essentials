namespace Franzo.Essentials;

public class InvalidAttributeDataException : Exception
{
    public InvalidAttributeDataException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
