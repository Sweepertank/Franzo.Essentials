using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Franzo.Essentials;

public class NetStandardNotifyPropertyChangedImpl :
    INotifyPropertyChanged,
    __ICanFirePropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void FirePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(propertyName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SetPropertyNotifying<T>(
        ref T field,
        T value,
        [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        FirePropertyChanged(propertyName);
    }

    protected virtual void OnPropertyChanged(string? propertyName) { }

    void __ICanFirePropertyChanged.__FirePropertyChanged(string? propertyName)
    {
        FirePropertyChanged(propertyName);
    }
}
