namespace Franzo.Essentials.Reflection;

public static class ObjectExtensions
{
    public static bool ValueIsAssignableTo(this object? self, NullableAwareType type)
    {
        return (self is null && type.IsNullable)
            || (self is not null && self.GetType().IsAssignableTo(type.Type));
    }

    public static string ValueTypeName(this object? self)
    {
        return self?.GetType().FullNameOrName() ?? "null";
    }
}
