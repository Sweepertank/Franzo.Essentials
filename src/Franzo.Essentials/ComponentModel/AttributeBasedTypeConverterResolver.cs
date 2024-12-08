using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Franzo.Essentials.ComponentModel;

public partial class AttributeBasedTypeConverterResolver : ITypeConverterResolver
{
    public static readonly AttributeBasedTypeConverterResolver Instance = new();

    bool ITypeConverterResolver.TryResolveTypeConverterCore(
        Type type,
        [NotNullWhen(true)] out TypeConverter? converter,
        ref Exception? error,
        Type? sourceType,
        Type? destinationType)
    {
        var converterType = type.AttributeSpecifiedTypeConverterType();
        if (converterType is null)
        {
            converter = null;
            return false;
        }

        converter = (TypeConverter)Activator.CreateInstance(converterType)!;
        return true;
    }
}
