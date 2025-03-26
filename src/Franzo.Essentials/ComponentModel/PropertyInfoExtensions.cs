using System.Reflection;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.ComponentModel;

public static class PropertyInfoExtensions
{
    /*public static bool HasMayChangeSilentlyAttribute(this PropertyInfo self)
    {
        return self.HasCustomAttribute<MayChangeSilentlyAttribute>();
    }*/

    public static MethodInfo? AttributeSpecifiedAddMethod(this PropertyInfo self)
    {
        var attribute = self.GetCustomAttributeBetter<AddMethodAttribute>();
        if (attribute is null)
        {
            return null;
        }

        NullableAwareType? itemType;
        try
        {
            itemType = self.PropertyType.IReadOnlyCollection_T_ItemType()?.ToNullableAware();
        }
        catch (AmbiguousMatchException ambiguousMatchError)
        {
            throw new InvalidAttributeDataException(
                $"Property '{self.Name}' on type '{self.DeclaringType}' has an {typeof(AddMethodAttribute).NameWithoutAttributeSuffix()} attribute, but the property's type, '{self.PropertyType}', implements {nameof(IEnumerable<object>)}<T> more than once.",
                ambiguousMatchError);
        }

        if (itemType is null)
        {
            throw new InvalidAttributeDataException(
                $"Property '{self.Name}' on type '{self.DeclaringType}' has an {typeof(AddMethodAttribute).NameWithoutAttributeSuffix()} attribute, but the property's type, '{self.PropertyType}', does not implement {nameof(IReadOnlyCollection<object>)}<T>.");
        }

        if (self.DeclaringType is null)
        {
            throw new InvalidAttributeDataException(
                $"Property '{self.Name}' has an {typeof(AddMethodAttribute).NameWithoutAttributeSuffix()} attribute, but the property's {nameof(PropertyInfo.DeclaringType)} is null.");
        }

        if (attribute.MethodName is null)
        {
            throw new InvalidAttributeDataException(
                $"MethodName of the {typeof(AddMethodAttribute).NameWithoutAttributeSuffix()} attribute applied to property '{self.Name}' on type '{self.DeclaringType}' is null.");
        }

        Type[] addMethodParameterTypes;
        if (self.PropertyType.ImplementsIReadOnlyDictionary_TKey_TValue())
        {
            addMethodParameterTypes = [
                itemType.GenericTypeArguments[0].Type,
                itemType.GenericTypeArguments[1].Type
            ];
        }
        else
        {
            addMethodParameterTypes = [itemType.Type];
        }

        var addMethod = self.DeclaringType.GetMethod(
            attribute.MethodName,
            0,
            ReflectionHelper.PublicInstanceBindingFlags,
            null,
            addMethodParameterTypes,
            null);

        if (addMethod is null)
        {
            throw new InvalidAttributeDataException(
                $"MethodName '{attribute.MethodName}' of the {typeof(AddMethodAttribute).NameWithoutAttributeSuffix()} attribute applied to property '{self.Name}' on type '{self.DeclaringType}' does not correspond to a valid, public {typeof(AddMethodAttribute).NameWithoutAttributeSuffix()} for that property on said type.");
        }

        return addMethod;
    }

    public static MethodInfo? AttributeSpecifiedInsertMethod(this PropertyInfo self)
    {
        var attribute = self.GetCustomAttributeBetter<InsertMethodAttribute>();
        if (attribute is null)
        {
            return null;
        }

        if (self.PropertyType.ImplementsIReadOnlyDictionary_TKey_TValue()
            || self.PropertyType.ImplementsIDictionary_TKey_TValue())
        {
            throw new InvalidAttributeDataException(
                $"Property '{self.Name}' on type '{self.DeclaringType}' has an {typeof(AddMethodAttribute).NameWithoutAttributeSuffix()} attribute, but the property's type, '{self.PropertyType}', implements {nameof(IReadOnlyDictionary<object, object>)}<T> and/or {nameof(IDictionary<object, object>)}<T>.");
        }

        NullableAwareType? itemType;
        try
        {
            itemType = self.PropertyType.IReadOnlyCollection_T_ItemType()?.ToNullableAware();
        }
        catch (AmbiguousMatchException ambiguousMatchError)
        {
            throw new InvalidAttributeDataException(
                $"Property '{self.Name}' on type '{self.DeclaringType}' has an {typeof(AddMethodAttribute).NameWithoutAttributeSuffix()} attribute, but the property's type, '{self.PropertyType}', implements {nameof(IEnumerable<object>)}<T> more than once.",
                ambiguousMatchError);
        }

        if (itemType is null)
        {
            throw new InvalidAttributeDataException(
                $"Property '{self.Name}' on type '{self.DeclaringType}' has an {typeof(AddMethodAttribute).NameWithoutAttributeSuffix()} attribute, but the property's type, '{self.PropertyType}', does not implement {nameof(IReadOnlyCollection<object>)}<T>.");
        }

        if (self.DeclaringType is null)
        {
            throw new InvalidAttributeDataException(
                $"Property '{self.Name}' has an {typeof(InsertMethodAttribute).NameWithoutAttributeSuffix()} attribute, but the property's {nameof(PropertyInfo.DeclaringType)} is null.");
        }

        if (attribute.MethodName is null)
        {
            throw new InvalidAttributeDataException(
                $"MethodName of the {typeof(InsertMethodAttribute).NameWithoutAttributeSuffix()} attribute applied to property '{self.Name}' on type '{self.DeclaringType}' is null.");
        }

        var insertMethod = self.DeclaringType.GetMethod(
            attribute.MethodName,
            0,
            ReflectionHelper.PublicInstanceBindingFlags,
            null,
            [typeof(int), itemType.Type],
            null);

        if (insertMethod is null)
        {
            throw new InvalidAttributeDataException(
                $"MethodName '{attribute.MethodName}' of the {typeof(InsertMethodAttribute).NameWithoutAttributeSuffix()} attribute applied to property '{self.Name}' on type '{self.DeclaringType}' does not correspond to a valid, public {typeof(InsertMethodAttribute).NameWithoutAttributeSuffix()} for that property on said type.");
        }

        return insertMethod;
    }
}
