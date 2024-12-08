namespace Franzo.Essentials.Reflection;

public static class TypeResolverExtensions
{
    public static ITypeResolver OrNullResolver(this ITypeResolver? self)
    {
        return self ?? ITypeResolver.Null;
    }
}
