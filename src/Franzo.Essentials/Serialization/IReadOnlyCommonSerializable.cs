using System.Reflection;

namespace Franzo.Essentials.Serialization;

public partial interface IReadOnlyCommonSerializable
{
    public static readonly PropertyInfo ItemsProperty =
        typeof(IReadOnlyCommonSerializable).GetProperty(
            nameof(_Items),
            BindingFlags.Static | BindingFlags.NonPublic)!;

    private static object? _Items
    {
        get => throw new InvalidOperationException();
    }

    /*public sealed bool IsPropertyTransient(PropertyOrFieldInfo property)
    {
        if (property.PropertyOrFieldType.HasTransientAttribute())
        {
            return true;
        }

        return IsPropertyTransientCore(property) ?? property.HasTransientAttribute();
    }

    public static bool? DefaultIsPropertyTransient(PropertyOrFieldInfo property)
    {
        return null;
    }

    protected bool? IsPropertyTransientCore(PropertyOrFieldInfo property)
    {
        return DefaultIsPropertyTransient(property);
    }*/

    public partial class Data_
    {
    }
}
