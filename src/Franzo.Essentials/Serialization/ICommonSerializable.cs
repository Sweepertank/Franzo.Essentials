namespace Franzo.Essentials.Serialization;

public partial interface ICommonSerializable : IReadOnlyCommonSerializable
{
    public void OnDeserializing() { }
    public void OnDeserialized() { }

    public new partial class Data_
    {
    }
}
