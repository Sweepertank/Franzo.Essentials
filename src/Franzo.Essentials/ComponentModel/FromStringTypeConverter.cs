using System.ComponentModel;
using System.Globalization;
using Franzo.Essentials;

namespace Franzo.Essentials.ComponentModel;

public abstract class FromStringTypeConverter<T> : BaseTypeConverter<T>
{
    public new T ConvertFromInvariantString(string str)
    {
        return ConvertFromStringCore(null, null, str);
    }

    public new T ConvertFromString(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        string str)
    {
        return ConvertFromStringCore(context, culture, str);
    }

    public string ConvertToString(T value)
    {
        return ConvertToStringCore(null, null, value);
    }

    public string ConvertToString(ITypeDescriptorContext? context, CultureInfo? culture, T value)
    {
        return ConvertToStringCore(context, culture, value);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override T ConvertFromGeneric(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value)
    {
        if (value is not string str)
        {
            // This cast would never be valid, but this call should never actually return
            return base.ConvertFromGeneric(context, culture, value)!;
        }

        return ConvertFromString(context, culture, str);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string);
    }

    public override object? ConvertToGeneric(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        T value,
        Type destinationType)
    {
        if (destinationType != typeof(string))
        {
            return base.ConvertToGeneric(context, culture, value, destinationType);
        }

        return ConvertToString(context, culture, value);
    }

    protected abstract T ConvertFromStringCore(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        string str);

    protected virtual string ConvertToStringCore(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        T value)
    {
        return value?.ToString() ?? throw new InvalidImplementationException();
    }
}
