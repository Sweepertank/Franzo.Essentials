namespace Franzo.Essentials.ComponentModel;

// @AfterBaseT
public abstract partial class PropertyChangedNotifier : IPropertyChangedNotifier
{
    protected virtual void OnPropertyChanged(string? propertyName) { }

    void IPropertyChangedNotifier.OnPropertyChanged(string? propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}
