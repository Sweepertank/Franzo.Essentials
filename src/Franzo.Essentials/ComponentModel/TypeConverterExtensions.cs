using System.ComponentModel;
using Franzo.Essentials;

namespace Franzo.Essentials.ComponentModel;

public static class TypeConverterExtensions
{
    public static object ConvertFromInvariantStringEnsuringAssignable(
        this TypeConverter self,
        string text,
        Type destinationType)
    {
        var converted = self.ConvertFromInvariantString(text);
        if (converted is null)
        {
            throw new InvalidImplementationException(
                $"The {nameof(TypeConverter.ConvertFromInvariantString)} method of {nameof(TypeConverter)} '{self.GetType()}' returned null.");
        }

        var convertedType = converted.GetType();
        if (!convertedType.IsAssignableTo(destinationType))
        {
            throw new InvalidImplementationException(
                $"The {nameof(TypeConverter.ConvertFromInvariantString)} method of {nameof(TypeConverter)} '{self.GetType()}' was expected to return a value of type '{destinationType}', but it instead returned a value of type '{convertedType}'.");
        }

        return converted;
    }

    public static string ConvertToInvariantStringEnsuringNotNull(
        this TypeConverter self,
        object? value)
    {
        var converted = self.ConvertToInvariantString(value);
        if (converted is null)
        {
            throw new InvalidImplementationException(
                $"The {nameof(TypeConverter.ConvertToInvariantString)} method of {nameof(TypeConverter)} '{self.GetType()}' was expected to return a string, but it instead returned null.");
        }

        return converted;
    }
}
