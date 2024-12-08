using System.Reflection;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.Serialization;

public static class ConstructorInfoExtensions
{
    public static bool HasDeserializationConstructorAttribute(this ConstructorInfo self)
    {
        return self.HasCustomAttribute<DeserializationConstructorAttribute>();
    }

    public static DeserializationConstructorAttribute? DeserializationConstructorAttribute(
        this ConstructorInfo self)
    {
        return self.GetCustomAttributeBetter<DeserializationConstructorAttribute>();
    }
}
