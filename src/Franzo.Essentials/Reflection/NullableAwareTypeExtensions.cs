namespace Franzo.Essentials.Reflection;

public static class NullableAwareTypeExtensions
{
    public static NullableAwareType OrNullableObjectType(this NullableAwareType? self)
    {
        return self ?? NullableAwareType.Of<object?>();
    }
}
