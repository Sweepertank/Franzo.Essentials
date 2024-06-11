namespace Franzo.Essentials.InterfaceInheritance;

public abstract class InterfaceData : NetStandardNotifyPropertyChangedImpl
{
    public bool __IsConstructed = false;

    internal InterfaceData()
    {
    }
}

public abstract class InterfaceData<T> : InterfaceData where T : class
{
    public T __This = null!;

    public T This
    {
        get => __This;
    }

    protected sealed override void OnPropertyChanged(string? propertyName)
    {
        if (This is __ICanFirePropertyChanged canFirePropertyChanged)
        {
            canFirePropertyChanged.__FirePropertyChanged(propertyName);
        }
    }
}
