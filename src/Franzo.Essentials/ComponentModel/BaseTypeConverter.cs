using System.ComponentModel;
using System.Globalization;

namespace Franzo.Essentials.ComponentModel;

public abstract class BaseTypeConverter : TypeConverter
{
    public abstract Type TypeCanConvert { get; }
}

public abstract class BaseTypeConverter<T> : BaseTypeConverter
{
    public sealed override Type TypeCanConvert
    {
        get => typeof(T);
    }

    public virtual T ConvertFromGeneric(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value)
    {
        return (T)base.ConvertFrom(context, culture, value)!;
    }

    public virtual object? ConvertToGeneric(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        T value,
        Type destinationType)
    {
        return base.ConvertTo(null, culture, value, destinationType);
    }

    public sealed override object? ConvertFrom(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value)
    {
        return ConvertFromGeneric(context, culture, value);
    }

    public sealed override object? ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object? value,
        Type destinationType)
    {
        if (value is not T castedValue)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        return ConvertToGeneric(context, culture, castedValue, destinationType);
    }
}
