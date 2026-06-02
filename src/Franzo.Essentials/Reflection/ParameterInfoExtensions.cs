using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class ParameterInfoExtensions
{
    public static bool HasCustomAttribute<T>(this ParameterInfo self) where T : Attribute
    {
        return self.GetCustomAttributes<T>().Any();
    }

    public static bool IsParams(this ParameterInfo self)
    {
        return self.HasCustomAttribute<ParamArrayAttribute>();
    }

    public static bool IsNullable(this ParameterInfo self)
    {
        return self.NullableAwareParameterType().IsNullable;
    }

    public static NullabilityInfo NullabilityInfo(this ParameterInfo self)
    {
        var context = new NullabilityInfoContext();
        return context.Create(self);
    }

    public static NullableAwareType NullableAwareParameterType(this ParameterInfo self)
    {
        return self.ParameterType.ToNullableAware(self.NullabilityInfo());
    }

    public static bool TypeIsEnumerableInterfaceOfHopefullyReadOnlyCollectionType(
        this ParameterInfo self,
        Type type)
    {
        return self.ParameterType.IsConstructedFromGenericType(typeof(IEnumerable<>))
            && type.ImplementsIReadOnlyCollection_T_ExactlyOnce()
            && self.ParameterType.IEnumerable_T_ItemType() == type.IReadOnlyCollection_T_ItemType();
    }
}
