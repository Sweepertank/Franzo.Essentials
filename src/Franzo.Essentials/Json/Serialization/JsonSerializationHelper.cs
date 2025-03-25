using System.Text;
using System.Text.Json;

namespace Franzo.Essentials.Json.Serialization;

public static class JsonSerializationHelper
{
    /*public static JsonSerializerOptions CreateOptions(ITypeResolver? typeResolver = null)
    {
        var options = new JsonSerializerOptions();
        options.WriteIndented = true;
        //options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new TypeJsonConverter(typeResolver.OrNullResolver()));

        return options;
    }

    public static void Serialize(
        Stream stream,
        object? value,
        ITypeResolver? typeResolver = null)
    {
        var options = CreateOptions(typeResolver);
        JsonSerializer.Serialize(stream, value, options);
    }

    public static string Serialize(object? value, ITypeResolver? typeResolver = null)
    {
        var options = CreateOptions(typeResolver);
        return JsonSerializer.Serialize(value, options);
    }*/

    public static object? Deserialize(
        Type returnType,
        string json,
        JsonSerializerOptions? options = null)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return Deserialize(returnType, stream, options);
    }

    public static object? DeserializeFromFile(
        Type returnType,
        string filePath,
        JsonSerializerOptions? options = null)
    {
        using var stream = File.OpenRead(filePath);
        return Deserialize(returnType, stream, options);
    }

    public static object? Deserialize(
        Type returnType,
        Stream stream,
        JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize(stream, returnType, options);
    }

    public static T DeserializeInstanceFromFile<T>(
        string filePath,
        JsonSerializerOptions? options = null)
    {
        var value = DeserializeFromFile(typeof(T), filePath, options);
        if (value is null)
        {
            throw CreateCannotDeserializeFromNullTokenException<T>();
        }

        return (T)value;
    }

    public static T DeserializeInstance<T>(Stream stream, JsonSerializerOptions? options = null)
    {
        var value = Deserialize(typeof(T), stream, options);
        if (value is null)
        {
            throw CreateCannotDeserializeFromNullTokenException<T>();
        }

        return (T)value;
    }

    public static T DeserializeInstance<T>(string json, JsonSerializerOptions? options = null)
    {
        var value = Deserialize(typeof(T), json, options);
        if (value is null)
        {
            throw CreateCannotDeserializeFromNullTokenException<T>();
        }

        return (T)value;
    }

    /*public static object? SerializeThenDeserialize(
        Type returnType,
        object? value,
        ITypeResolver? typeResolver = null)
    {
        var jsonString = Serialize(value, typeResolver);
        return Deserialize(returnType, jsonString, typeResolver);
    }*/

    private static JsonException CreateCannotDeserializeFromNullTokenException<T>()
    {
        return new JsonException(
            $"Cannot deserialize {typeof(T)} from JSON consisting of a single null token.");
    }
}
