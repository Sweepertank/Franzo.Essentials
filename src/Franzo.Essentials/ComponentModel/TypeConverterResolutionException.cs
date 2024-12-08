namespace Franzo.Essentials.ComponentModel;

public class TypeConverterResolutionException : Exception
{
    public TypeConverterResolutionException(
        string? message = null,
        Exception? innerException = null) : base(message, innerException)
    {
    }
}
