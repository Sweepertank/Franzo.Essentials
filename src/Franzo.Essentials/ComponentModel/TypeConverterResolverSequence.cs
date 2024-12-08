using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Franzo.Essentials.ComponentModel;

public partial class TypeConverterResolverSequence : ITypeConverterResolver
{
    public IEnumerable<ITypeConverterResolver> Resolvers { get; }

    public TypeConverterResolverSequence(IEnumerable<ITypeConverterResolver> resolvers)
    {
        Resolvers = resolvers;
    }

    bool ITypeConverterResolver.TryResolveTypeConverterCore(
        Type type,
        [NotNullWhen(true)] out TypeConverter? converter,
        ref Exception? error,
        Type? sourceType,
        Type? destinationType)
    {
        foreach (var resolver in Resolvers)
        {
            if (resolver.TryResolveTypeConverter(
                type,
                out converter,
                sourceType,
                destinationType))
            {
                return true;
            }
        }

        converter = null;
        return false;
    }
}
