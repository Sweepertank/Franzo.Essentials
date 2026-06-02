using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Franzo.Essentials.ComponentModel;

public partial interface IPropertyChangedNotifier : INotifyPropertyChanged
{
    public new sealed event PropertyChangedEventHandler? PropertyChanged
    {
        add => PropertyChangedCore += value;
        remove => PropertyChangedCore -= value;
    }

    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add => PropertyChangedCore += value;
        remove => PropertyChangedCore -= value;
    }

    // @AfterBaseT: methods should be protected
    public sealed void FirePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(propertyName);
        PropertyChangedCore?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public sealed void SetPropertyNotifying<T>(
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

    protected void OnPropertyChanged(string? propertyName) { }

    public partial class Data_
    {
        private PropertyChangedEventHandler? PropertyChangedCore { get; set; } = null;

        public Data_()
        {
        }
    }
}
