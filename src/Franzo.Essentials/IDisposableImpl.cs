namespace Franzo.Essentials;

public partial interface IDisposableImpl : IDisposable, ICheckIfDisposed, INotifyDisposed
{
    bool ICheckIfDisposed.IsDisposed
    {
        get => IsDisposed;
    }

    protected void DisposeCore();

    void IDisposable.Dispose()
    {
        lock (DisposeLock)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
        }

        DisposeCore();
        NotifyDisposed();
    }

    public new partial class Data_
    {
        internal bool IsDisposed { get; set; } = false;
        internal object DisposeLock { get; } = new();
    }
}
