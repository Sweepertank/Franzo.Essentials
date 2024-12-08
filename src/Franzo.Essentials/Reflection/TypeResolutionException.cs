namespace Franzo.Essentials.Reflection;

public class TypeResolutionException : Exception
{
    public TypeResolutionException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
