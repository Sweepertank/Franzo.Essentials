namespace Franzo.Essentials;

public static class ErrorHandlerHelper
{
    public static void NullErrorHandler(this Exception error)
    {
        throw error;
    }
}
