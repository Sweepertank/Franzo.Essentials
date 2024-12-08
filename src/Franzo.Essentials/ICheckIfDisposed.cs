namespace Franzo.Essentials;

public partial interface ICheckIfDisposed
{
    public bool IsDisposed { get; }

    // @AfterBaseT: protected
    public sealed void ThrowIfDisposed(Func<Exception> exceptionFactory)
    {
        if (IsDisposed)
        {
            throw exceptionFactory.Invoke();
        }
    }

    public sealed void ThrowIfDisposed(string? objectName, string? message = null)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(
                objectName ?? GetType().Name,
                message ?? "The operation is not valid because the object has been disposed.");
        }
    }
}
