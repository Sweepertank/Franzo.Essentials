using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class CustomAttributeProviderExtensions
{
    public static Type ReturnType(this ICustomAttributeProvider self)
    {
        return self switch
        {
            PropertyInfo property => property.PropertyType,
            FieldInfo field => field.FieldType,
            MethodInfo method => method.ReturnType,
            EventInfo @event => @event.EventHandlerType ?? throw new ShouldNeverBeThrownException(),
            ParameterInfo parameter => parameter.ParameterType,
            _ => throw new ArgumentException(
                $"The given {nameof(ICustomAttributeProvider)} must be either a property, field, method, event, or parameter. Instead, however, the given {nameof(ICustomAttributeProvider)} was of type '{self.GetType()}'.",
                nameof(self))
        };
    }

    public static NullabilityInfo NullabilityInfo(this ICustomAttributeProvider self)
    {
        return self switch
        {
            PropertyInfo property => property.NullabilityInfo(),
            FieldInfo field => field.NullabilityInfo(),
            EventInfo @event => @event.NullabilityInfo(),
            ParameterInfo parameter => parameter.NullabilityInfo(),
            _ => throw new ArgumentException(
                $"The given {nameof(ICustomAttributeProvider)} must be either a property, field, event, or parameter. Instead, however, the given {nameof(ICustomAttributeProvider)} was of type '{self.GetType()}'.",
                nameof(self))
        };
    }

    public static NullableAwareType NullableAwareReturnType(this ICustomAttributeProvider self)
    {
        return self.ReturnType().ToNullableAware(self.NullabilityInfo());
    }
}
