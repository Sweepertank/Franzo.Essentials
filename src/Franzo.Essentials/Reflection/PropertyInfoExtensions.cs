using System.Reflection;
using Franzo.Essentials;
using Franzo.Essentials.ComponentModel;

namespace Franzo.Essentials.Reflection;

public static class PropertyInfoExtensions
{
    public static bool IsStatic(this PropertyInfo self)
    {
        return self.GetMethod?.IsStatic
            ?? self.SetMethod?.IsStatic
            ?? throw new ShouldNeverBeThrownException();
    }

    public static bool IsNullable(this PropertyInfo self)
    {
        return self.NullableAwarePropertyType().IsNullable;
    }

    public static NullabilityInfo NullabilityInfo(this PropertyInfo self)
    {
        var context = new NullabilityInfoContext();
        return context.Create(self);
    }

    public static NullableAwareType NullableAwarePropertyType(this PropertyInfo self)
    {
        return self.PropertyType.ToNullableAware(self.NullabilityInfo());
    }

    public static object? GetValueBetter(
        this PropertyInfo self,
        object? target,
        bool failIfNoPublicGetter = false)
    {
        if (self.GetMethod is null)
        {
            throw new MethodAccessException(
                $"Cannot get value of the given property '{self.Name}' on type '{self.DeclaringType}' because it does not have a get method.");
        }
        else if (!self.GetMethod.IsPublic && failIfNoPublicGetter)
        {
            throw new MethodAccessException(
                $"Cannot get value of the given property '{self.Name}' on type '{self.DeclaringType}' because it does not have a public get method.");
        }

        return ComponentReflectionHelper.CallGetMethod(target, self.GetMethod);
    }

    public static void SetValueBetter(
        this PropertyInfo self,
        object? target,
        object? value,
        bool failIfSetterNotPublic = false)
    {
        if (self.SetMethod is null)
        {
            throw new MethodAccessException(
                $"Cannot set value of the given property '{self.Name}' on type '{self.DeclaringType}' because it does not have a set method.");
        }
        else if (!self.SetMethod.IsPublic && failIfSetterNotPublic)
        {
            throw new MethodAccessException(
                $"Cannot set value of the given property '{self.Name}' on type '{self.DeclaringType}' because it does not have a public set method.");
        }

        ComponentReflectionHelper.CallSetMethod(target, self.SetMethod, value);
    }

    public static bool HasPublicGetMethod(this PropertyInfo self)
    {
        return self.GetMethod.ExistsAndIsPublic();
    }

    public static bool HasPublicSetMethod(this PropertyInfo self)
    {
        return self.SetMethod.ExistsAndIsPublic();
    }

    [Obsolete]
    public static bool IsHiddenByPropertyOn(this PropertyInfo self, Type type)
    {
        if (self.DeclaringType is null
            || self.DeclaringType.IsInterface
            || !type.IsAssignableTo(self.DeclaringType))
        {
            return false;
        }

        var propertyOfSameName = type.GetProperty(self.Name);
        return !propertyOfSameName.MemberEquals(self);
    }
}
