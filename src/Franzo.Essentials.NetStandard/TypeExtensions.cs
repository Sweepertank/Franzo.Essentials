namespace Franzo.Essentials;

public static class TypeExtensions
{
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
