namespace Franzo.Essentials.ComponentModel;

public static class TypeConverterResolverExtensions
{
    public static ITypeConverterResolver OrNullResolver(this ITypeConverterResolver? self)
    {
        return self ?? ITypeConverterResolver.Null;
    }
}
