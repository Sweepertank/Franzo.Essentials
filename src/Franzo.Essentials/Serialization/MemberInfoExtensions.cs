using System.Reflection;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.Serialization;

public static class MemberInfoExtensions
{
    public static bool IsSerialized(
        this MemberInfo self,
        bool useOptInPropertySerialization = false)
    {
        if (self.DeclaringType?.HasPropertiesAreTransientByDefaultAttribute() ?? false)
        {
            return self.HasSerializedAttribute();
        }
        else
        {
            return (useOptInPropertySerialization && self.HasSerializedAttribute())
                || (!useOptInPropertySerialization && !self.HasTransientAttribute());
        }
    }

    public static bool HasSerializedAttribute(this MemberInfo self)
    {
        return self.HasCustomAttribute<SerializedAttribute>();
    }

    public static bool HasTransientAttribute(this MemberInfo self)
    {
        return self.HasCustomAttribute<TransientAttribute>();
    }

    public static bool IsContentProperty(this MemberInfo self)
    {
        return self.HasCustomAttribute<ContentPropertyAttribute>();
    }

    public static bool IsItemsProperty(this MemberInfo self)
    {
        return self.MemberEquals(ICommonSerializable.ItemsProperty);
    }
}
