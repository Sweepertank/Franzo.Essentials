using System.Diagnostics.CodeAnalysis;

namespace Franzo.Essentials.Reflection;

public interface ITypeResolver
{
    public static readonly ITypeResolver Null = NullTypeResolver.Instance;

    public sealed Type ResolveType(
        string typeName,
        TypeNameStyle typeNameStyle = TypeNameStyle.Unspecified)
    {
        if (TryResolveType(typeName, out var type, out var error, typeNameStyle))
        {
            return type;
        }

        throw error;
    }

    public sealed bool TryResolveType(
        string typeName,
        [NotNullWhen(true)] out Type? type,
        [NotNullWhen(false)] out TypeResolutionException? error,
        TypeNameStyle typeNameStyle = TypeNameStyle.Unspecified)
    {
        Exception? refError = null;
        if (!TryResolveTypeCore(typeName, out type, ref refError, typeNameStyle))
        {
            if (refError is not TypeResolutionException typeResolutionError)
            {
                typeResolutionError = new TypeResolutionException(
                    $"Unable to resolve type '{typeName}' through {nameof(ITypeResolver)} of type {GetType()}.",
                    refError);
            }

            error = typeResolutionError;
            return false;
        }

        error = null;
        return true;
    }

    public sealed bool TryResolveType(
        string typeName,
        [NotNullWhen(true)] out Type? type,
        TypeNameStyle typeNameStyle = TypeNameStyle.Unspecified)
    {
        return TryResolveType(typeName, out type, out _, typeNameStyle);
    }

    protected bool TryResolveTypeCore(
        string typeName,
        [NotNullWhen(true)] out Type? type,
        ref Exception? error,
        TypeNameStyle typeNameStyle = TypeNameStyle.Unspecified);

    // @todo fix once core.serialization is gone
    public sealed string ResolveTypeName(
        Type type,
        TypeNameStyle style = TypeNameStyle.Unspecified)
    {
        return ResolveTypeNameCore(type, style);
    }

    protected string ResolveTypeNameCore(Type type, TypeNameStyle style)
    {
        return style switch
        {
            TypeNameStyle.Simple => type.Name,
            TypeNameStyle.Full => type.FullNameOrName(),
            TypeNameStyle.AssemblyQualified or TypeNameStyle.Unspecified
                => type.AssemblyQualifiedName ?? type.FullNameOrName(),
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };
    }

    public sealed TypeResolverSequence FollowedBy(ITypeResolver resolver)
    {
        return new TypeResolverSequence(this, resolver);
    }
}
