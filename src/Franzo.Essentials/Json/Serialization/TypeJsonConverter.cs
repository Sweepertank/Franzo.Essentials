using System.Text.Json;
using System.Text.Json.Serialization;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.Json.Serialization;

public class TypeJsonConverter : JsonConverter<Type>
{
    public ITypeResolver TypeResolver { get; }

    public TypeJsonConverter(ITypeResolver typeResolver)
    {
        TypeResolver = typeResolver;
    }

    public override Type? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is not string str)
        {
            return null;
        }

        if (!TypeResolver.TryResolveType(str, out var type, out var error))
        {
            throw new JsonException(null, error);
        }

        return type;
    }

    public override void Write(
        Utf8JsonWriter writer,
        Type value,
        JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(TypeResolver.ResolveTypeName(value));
        }
    }
}
