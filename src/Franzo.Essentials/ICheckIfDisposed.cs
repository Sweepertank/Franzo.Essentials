namespace Franzo.Essentials;

public partial interface ICheckIfDisposed
{
    public bool IsDisposed { get; }

    protected sealed void ThrowIfDisposed(Func<Exception> exceptionFactory)
    {
        if (IsDisposed)
        {
            throw exceptionFactory.Invoke();
        }
    }

    protected sealed void ThrowIfDisposed(string? objectName, string? message = null)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(
                objectName ?? GetType().Name,
                message ?? "The operation is not valid because the object has been disposed.");
        }
    }
}
