using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Franzo.Essentials.Reflection;

public partial class GlobalTypeResolver : ITypeResolver
{
    public static readonly GlobalTypeResolver Instance = new();

    bool ITypeResolver.TryResolveTypeCore(
        string typeName,
        [NotNullWhen(true)] out Type? type,
        ref Exception? error,
        TypeNameStyle typeNameStyle)
    {
        if (typeNameStyle is not (TypeNameStyle.AssemblyQualified or TypeNameStyle.Unspecified))
        {
            type = null;
            return false;
        }

        try
        {
            type = Type.GetType(typeName, throwOnError: true);
        }
        catch (Exception e) when (e is not FileNotFoundException)
        {
            type = null;
            error = e;
            return false;
        }

        Debug.Assert(type is not null);
        return true;
    }
}
