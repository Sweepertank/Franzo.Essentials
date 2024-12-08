using Franzo.Essentials.Serialization;

namespace Franzo.Essentials.ComponentModel;

public partial interface IFranzoComponent :
    IReadOnlyFranzoComponent,
    IPropertyChangedNotifier,
    IDisposableImpl,
    ICommonSerializable
{
    public new partial class Data_
    {
    }
}
