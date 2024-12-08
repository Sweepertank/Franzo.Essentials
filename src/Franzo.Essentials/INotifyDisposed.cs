namespace Franzo.Essentials;

public partial interface INotifyDisposed
{
    public sealed event EventHandler Disposed
    {
        add => DisposedCore += value;
        remove => DisposedCore -= value;
    }

    protected void NotifyDisposed()
    {
        DisposedCore?.Invoke(this, EventArgs.Empty);
    }

    public partial class Data_
    {
        private EventHandler? DisposedCore { get; set; } = null;
    }
}
