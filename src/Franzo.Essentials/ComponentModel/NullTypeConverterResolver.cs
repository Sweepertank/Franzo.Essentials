using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Franzo.Essentials.ComponentModel;

internal partial class NullTypeConverterResolver : ITypeConverterResolver
{
    public static readonly NullTypeConverterResolver Instance = new();

    bool ITypeConverterResolver.TryResolveTypeConverterCore(
        Type type,
        [NotNullWhen(true)] out TypeConverter? converter,
        ref Exception? error,
        Type? sourceType,
        Type? destinationType)
    {
        converter = null;
        return false;
    }
}
