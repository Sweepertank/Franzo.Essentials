namespace Franzo.Essentials.InterfaceInheritance;

public abstract class InterfaceData : NetStandardNotifyPropertyChangedImpl
{
    public object? __This;
    public bool __IsConstructed;

    public object This
    {
        get => __This!;
    }

    protected InterfaceData()
    {
        __IsConstructed = true;
    }

    protected sealed override void OnPropertyChanged(string? propertyName)
    {
        if (__This is __ICanFirePropertyChanged canFirePropertyChanged)
        {
            canFirePropertyChanged.__FirePropertyChanged(propertyName);
        }
    }
}
