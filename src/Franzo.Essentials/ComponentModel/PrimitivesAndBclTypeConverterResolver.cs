using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.ComponentModel;

public partial class PrimitivesAndBclTypeConverterResolver : ITypeConverterResolver
{
    public ITypeConverterResolver? NullableUnderlyingTypeConverterResolver { get; }

    public PrimitivesAndBclTypeConverterResolver(
        ITypeConverterResolver? nullableUnderlyingTypeConverterResolver = null)
    {
        NullableUnderlyingTypeConverterResolver = nullableUnderlyingTypeConverterResolver;
    }

    public static bool TryResolveTypeConverterStatic(
        Type type,
        [NotNullWhen(true)] out TypeConverter? converter,
        Type? sourceType = null,
        Type? destinationType = null,
        ITypeConverterResolver? nullableUnderlyingTypeConverterResolver = null)
    {
        var resolver = new PrimitivesAndBclTypeConverterResolver(
            nullableUnderlyingTypeConverterResolver);
        return resolver.TryResolveTypeConverter(type, out converter, sourceType, destinationType);
    }

    bool ITypeConverterResolver.TryResolveTypeConverterCore(
        Type type,
        [NotNullWhen(true)] out TypeConverter? converter,
        ref Exception? error,
        Type? sourceType,
        Type? destinationType)
    {
        // we have to do this because, for some reason, when you're running within MSBuild,
        // you can't resolve converters for Nullable types through TypeDescriptor
        if (type.IsNullableValueType())
        {
            var underlyingType = type.GenericTypeArguments[0];
            TypeConverter? underlyingTypeConverter;

            if (!ExcludingNullableResolver.Instance.TryResolveTypeConverter(
                underlyingType,
                out underlyingTypeConverter,
                sourceType,
                destinationType)
                && !NullableUnderlyingTypeConverterResolver
                    .OrNullResolver()
                    .TryResolveTypeConverter(
                        underlyingType,
                        out underlyingTypeConverter,
                        sourceType,
                        destinationType))
            {
                converter = null;
                return false;
            }

            var nullableConverter = new FranzoNullableConverter(type)
            {
                UnderlyingTypeConverter = underlyingTypeConverter
            };

            converter = nullableConverter;
            return true;
        }
        else if (ExcludingNullableResolver.Instance.TryResolveTypeConverter(
            type,
            out converter,
            sourceType,
            destinationType))
        {
            return true;
        }

        converter = null;
        return false;
    }

    private partial class ExcludingNullableResolver : ITypeConverterResolver
    {
        public static readonly ExcludingNullableResolver Instance = new();

        bool ITypeConverterResolver.TryResolveTypeConverterCore(
            Type type,
            [NotNullWhen(true)] out TypeConverter? converter,
            ref Exception? error,
            Type? sourceType,
            Type? destinationType)
        {
            // @todo: still a few more types we should probably add, see https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel
            if (type.IsEnum)
            {
                converter = new EnumConverter(type);
            }
            else if (type == typeof(string))
            {
                converter = new StringConverter();
            }
            else if (type == typeof(bool))
            {
                converter = new BooleanConverter();
            }
            else if (type == typeof(byte))
            {
                converter = new ByteConverter();
            }
            else if (type == typeof(sbyte))
            {
                converter = new SByteConverter();
            }
            else if (type == typeof(char))
            {
                converter = new CharConverter();
            }
            else if (type == typeof(decimal))
            {
                converter = new DecimalConverter();
            }
            else if (type == typeof(double))
            {
                converter = new DoubleConverter();
            }
            else if (type == typeof(float))
            {
                converter = new SingleConverter();
            }
            else if (type == typeof(int))
            {
                converter = new Int32Converter();
            }
            else if (type == typeof(uint))
            {
                converter = new UInt32Converter();
            }
            else if (type == typeof(long))
            {
                converter = new Int64Converter();
            }
            else if (type == typeof(ulong))
            {
                converter = new UInt64Converter();
            }
            else if (type == typeof(short))
            {
                converter = new Int16Converter();
            }
            else if (type == typeof(ushort))
            {
                converter = new UInt16Converter();
            }
            else if (type == typeof(Guid))
            {
                converter = new GuidConverter();
            }
            else
            {
                converter = null;
            }

            return converter is not null;
        }
    }
}
