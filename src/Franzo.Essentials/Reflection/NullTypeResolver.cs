using System.Diagnostics.CodeAnalysis;

namespace Franzo.Essentials.Reflection;

internal partial class NullTypeResolver : ITypeResolver
{
    public static NullTypeResolver Instance = new();

    bool ITypeResolver.TryResolveTypeCore(
        string typeName,
        [NotNullWhen(true)] out Type? type,
        ref Exception? error,
        TypeNameStyle typeNameStyle)
    {
        type = null;
        return false;
    }
}
