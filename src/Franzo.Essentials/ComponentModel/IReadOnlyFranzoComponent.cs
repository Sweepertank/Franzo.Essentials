using System.ComponentModel;
using Franzo.Essentials.Serialization;

namespace Franzo.Essentials.ComponentModel;

public partial interface IReadOnlyFranzoComponent :
    INotifyPropertyChanged,
    IReadOnlyCommonSerializable,
    ICheckIfDisposed,
    INotifyDisposed
{
    public new partial class Data_
    {
    }
}
