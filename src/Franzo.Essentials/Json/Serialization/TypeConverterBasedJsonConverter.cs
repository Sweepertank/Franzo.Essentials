using System.Text.Json;
using System.Text.Json.Serialization;
using Franzo.Essentials.ComponentModel;

namespace Franzo.Essentials.Json.Serialization;

public abstract class TypeConverterBasedJsonConverter<T> : JsonConverter<T>
{
    protected abstract FromStringTypeConverter<T> TypeConverter { get; }

    public override T? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (!reader.TryGetNonNullString(out var str))
        {
            throw new JsonException();
        }

        try
        {
            // reader.Value may be null, but we want to trigger the TypeConverter's exception for that
            return TypeConverter.ConvertFromInvariantString(str);
        }
        catch (Exception e)
        {
            throw new JsonException(null, e);
        }
    }

    public override void Write(
        Utf8JsonWriter writer,
        T value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(TypeConverter.ConvertToString(value));
    }
}
