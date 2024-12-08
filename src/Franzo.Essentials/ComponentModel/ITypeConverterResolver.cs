using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Franzo.Essentials.ComponentModel;

public partial interface ITypeConverterResolver
{
    public static readonly ITypeConverterResolver Null = NullTypeConverterResolver.Instance;

    public sealed TypeConverter ResolveTypeConverter(
        Type type,
        Type? sourceType = null,
        Type? destinationType = null)
    {
        if (!TryResolveTypeConverter(
            type,
            out var converter,
            out var error,
            sourceType,
            destinationType))
        {
            throw error;
        }

        return converter;
    }

    public sealed bool TryResolveTypeConverter(
        Type type,
        [NotNullWhen(true)] out TypeConverter? converter,
        [NotNullWhen(false)] out TypeConverterResolutionException? error,
        Type? sourceType = null,
        Type? destinationType = null)
    {
        Exception? refError = null;
        var result = TryResolveTypeConverterCore(
            type,
            out converter,
            ref refError,
            sourceType,
            destinationType)
            && (sourceType is null || converter.CanConvertFrom(sourceType))
            && (destinationType is null || converter.CanConvertTo(destinationType));

        if (!result)
        {
            if (refError is not TypeConverterResolutionException converterResolutionError)
            {
                converterResolutionError = new TypeConverterResolutionException(
                    $"Unable to resolve type converter for type '{type}' through {nameof(ITypeConverterResolver)} of type {GetType()}.");
            }

            error = converterResolutionError;
            return false;
        }

        Debug.Assert(converter is not null);
        error = null;
        return true;
    }

    public sealed bool TryResolveTypeConverter(
        Type type,
        [NotNullWhen(true)] out TypeConverter? converter,
        Type? sourceType = null,
        Type? destinationType = null)
    {
        return TryResolveTypeConverter(type, out converter, out _, sourceType, destinationType);
    }

    protected bool TryResolveTypeConverterCore(
        Type type,
        [NotNullWhen(true)] out TypeConverter? converter,
        ref Exception? error,
        Type? sourceType,
        Type? destinationType);
}
