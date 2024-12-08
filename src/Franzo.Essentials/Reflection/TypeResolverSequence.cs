using System.Diagnostics.CodeAnalysis;

namespace Franzo.Essentials.Reflection;

public partial class TypeResolverSequence : ITypeResolver
{
    public IEnumerable<ITypeResolver> Resolvers { get; }

    public TypeResolverSequence(IEnumerable<ITypeResolver> resolvers)
    {
        Resolvers = resolvers;
    }

    public TypeResolverSequence(params ITypeResolver[] resolvers)
        : this((IEnumerable<ITypeResolver>)resolvers)
    {
    }

    bool ITypeResolver.TryResolveTypeCore(
        string typeName,
        [NotNullWhen(true)] out Type? type,
        ref Exception? error,
        TypeNameStyle typeNameStyle)
    {
        foreach (var resolver in Resolvers)
        {
            if (resolver.TryResolveType(typeName, out type, out _, typeNameStyle))
            {
                return true;
            }
        }

        type = null;
        return false;
    }
}
