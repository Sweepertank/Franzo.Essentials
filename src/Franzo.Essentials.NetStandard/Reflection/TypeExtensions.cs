namespace Franzo.Essentials.Reflection;

public static class TypeExtensions
{
    /*public static PropertyInfo GetPropertyOrThrow(
        this Type self,
        string propertyName,
        BindingFlags bindingFlags = NetStandardReflectionHelper.DefaultBindingFlags)
    {
        var property = self.GetProperty(propertyName, bindingFlags);
        if (property is null)
        {
            throw new InvalidOperationException(
                $"No property with name '{propertyName}' could not be found on the given type '{self}' using the given binding flags.");
        }

        return property;
    }*/

    public static string UngenericizedName(this Type self)
    {
        return self.IsGenericType
            ? self.Name.Substring(0, self.Name.Length - 2)
            : self.Name;
    }

    public static string FullUngenericizedName(this Type self)
    {
        return $"{self.Namespace}{Type.Delimiter}{self.UngenericizedName()}";
    }
}
