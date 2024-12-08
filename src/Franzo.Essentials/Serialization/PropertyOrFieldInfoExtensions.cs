using Franzo.Essentials;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.Serialization;

public static class PropertyOrFieldInfoExtensions
{
    public static bool IsSerialized(
        this PropertyOrFieldInfo self,
        bool useOptInPropertySerialization = false)
    {
        return self.Member.IsSerialized(useOptInPropertySerialization);
    }

    public static bool HasSerializedAttribute(this PropertyOrFieldInfo self)
    {
        return self.Member.HasSerializedAttribute();
    }

    public static bool HasTransientAttribute(this PropertyOrFieldInfo self)
    {
        return self.Member.HasTransientAttribute();
    }

    public static bool IsItemsProperty(this PropertyOrFieldInfo self)
    {
        return self.Member.IsItemsProperty();
    }

    public static bool IsContentProperty(this PropertyOrFieldInfo self)
    {
        return self.Member.IsContentProperty();
    }

    // Should only be called if type is known to have only 1 ReadOnlyCollection interface if self is ItemsProperty
    public static NullableAwareType PropertyOrFieldTypeAccountingForItemsProperty(
        this PropertyOrFieldInfo self,
        Type ownerType)
    {
        if (self.IsItemsProperty())
        {
            var collectionInterface = ownerType.IReadOnlyCollection_T_Interface();
            if (collectionInterface is null)
            {
                throw new ShouldNeverBeThrownException();
            }

            return collectionInterface.ToNullableAware();
        }
        else
        {
            return self.NullableAwarePropertyOrFieldType;
        }
    }
}
