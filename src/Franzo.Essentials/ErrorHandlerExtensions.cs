namespace Franzo.Essentials;

public static class ErrorHandlerExtensions
{
    public static ErrorHandler OrNullHandler(this ErrorHandler? self)
    {
        return self ?? ErrorHandlerHelper.NullErrorHandler;
    }
}
