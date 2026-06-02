using System.ComponentModel;
using System.Reflection;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.ComponentModel;

public static class MemberInfoExtensions
{
    public static object? ComponentModelExplicitDefaultValue(this MemberInfo self)
    {
        var defaultValueAttribute =
            self.GetCustomAttributeSearchingBaseInterfacesIfInterface<DefaultValueAttribute>();
        var defaultValueFieldAttribute =
            self.GetCustomAttributeSearchingBaseInterfacesIfInterface<DefaultValueFieldAttribute>();

        if (defaultValueAttribute is not null)
        {
            return defaultValueAttribute.Value;
        }
        else if (defaultValueFieldAttribute is not null)
        {
            if (self.DeclaringType is null)
            {
                throw new InvalidAttributeDataException(
                    $"Member '{self.Name}' has a {typeof(DefaultValueFieldAttribute).NameWithoutAttributeSuffix()} attribute, but the member's {nameof(MemberInfo.DeclaringType)} is null.");
            }

            var defaultValueField = self.DeclaringType.GetFieldBetter(
                defaultValueFieldAttribute.FieldName);
            if (defaultValueField is null)
            {
                throw new InvalidAttributeDataException(
                    $"{nameof(DefaultValueFieldAttribute.FieldName)} of the {typeof(DefaultValueFieldAttribute).NameWithoutAttributeSuffix()} attribute applied to member '{self.Name}' on type '{self.DeclaringType}' refers to a field, '{defaultValueFieldAttribute.FieldName}', which does not exist, or is not publicly visible, on said type.");
            }
            else if (!defaultValueField.IsStatic)
            {
                throw new InvalidAttributeDataException(
                    $"{nameof(DefaultValueFieldAttribute.FieldName)} of the {typeof(DefaultValueFieldAttribute).NameWithoutAttributeSuffix()} attribute applied to member '{self.Name}' on type '{self.DeclaringType}' refers to a field, '{defaultValueField.Name}', which is not static.");
            }

            return defaultValueField.GetValueBetter(null);
        }
        else
        {
            return null;
        }
    }

    public static string? AttributeSpecifiedTypeConverterTypeName(this MemberInfo self)
    {
        var attribute = self.TypeConverterAttribute();
        if (attribute is null)
        {
            return null;
        }

        if (attribute.ConverterTypeName is null)
        {
            throw new InvalidAttributeDataException(
                $"{nameof(System.ComponentModel.TypeConverterAttribute.ConverterTypeName)} of the {typeof(TypeConverterAttribute).NameWithoutAttributeSuffix()} attribute applied to member '{self}' is null.");
        }

        return attribute.ConverterTypeName;
    }

    public static Type? AttributeSpecifiedTypeConverterType(this MemberInfo self)
    {
        var converterTypeName = self.AttributeSpecifiedTypeConverterTypeName();
        if (converterTypeName is null)
        {
            return null;
        }

        var type = Type.GetType(converterTypeName);
        if (type is null)
        {
            throw new InvalidAttributeDataException(
                $"{nameof(System.ComponentModel.TypeConverterAttribute.ConverterTypeName)} '{converterTypeName}' of the {typeof(TypeConverterAttribute).NameWithoutAttributeSuffix()} attribute applied to member '{self}' does not correspond to a type in any currently loaded assembly.");
        }
        else if (!type.IsAssignableTo(typeof(TypeConverter)))
        {
            throw new InvalidAttributeDataException(
                $"The type '{type}' specified in the {typeof(TypeConverterAttribute).NameWithoutAttributeSuffix()} attribute applied to member '{self}' is not derived from {nameof(TypeConverter)}.");
        }

        return type;
    }

    public static TypeConverterAttribute? TypeConverterAttribute(this MemberInfo self)
    {
        return self.GetCustomAttributeSearchingBaseInterfacesIfInterface<TypeConverterAttribute>();
    }
}
