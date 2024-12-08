using System.Reflection;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.Serialization;

public static class TypeExtensions
{
    public static ConstructorInfo? AttributeSpecifiedDeserializationConstructor(this Type self)
    {
        return self.GetConstructors()
            .FirstOrDefault(c => c.HasDeserializationConstructorAttribute());
    }

    public static PropertyOrFieldInfo? AttributeSpecifiedContentProperty(this Type self)
    {
        foreach (var type in self.AncestorTypes())
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            PropertyOrFieldInfo? contentProperty = null;

            foreach (var declaredPropertyOrField in type.GetPropertiesAndFields(bindingFlags))
            {
                if (declaredPropertyOrField.IsContentProperty())
                {
                    if (contentProperty is not null)
                    {
                        throw new InvalidAttributeDataException(
                            $"Type '{self}' has a {typeof(ContentPropertyAttribute).NameWithoutAttributeSuffix()} attribute applied to more than one of its declared properties or fields.");
                    }

                    contentProperty = declaredPropertyOrField;
                }
            }

            if (contentProperty is not null)
            {
                return contentProperty;
            }
        }

        return null;
    }

    public static PropertyOrFieldInfo? ContentProperty(this Type self)
    {
        var attributeSpecifiedContentProperty = self.AttributeSpecifiedContentProperty();
        if (attributeSpecifiedContentProperty is not null)
        {
            return attributeSpecifiedContentProperty;
        }

        return self.ImplementsIReadOnlyCollection_T_ExactlyOnce()
            ? new PropertyOrFieldInfo(ICommonSerializable.ItemsProperty)
            : null;
    }

    public static bool HasPropertiesAreTransientByDefaultAttribute(this Type self)
    {
        return self.HasCustomAttributeSearchingBaseInterfacesIfInterface<PropertiesAreTransientByDefaultAttribute>();
    }

    public static IEnumerable<PropertyOrFieldInfo> GetSerializedPropertiesAndFields(
        this Type self,
        bool useOptInPropertySerialization = false)
    {
        return self.GetInstancePropertiesAndFields()
            .Where(p => p.IsSerialized(useOptInPropertySerialization));
    }
}
