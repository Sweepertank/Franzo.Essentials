namespace Franzo.Essentials.Roslyn;

public static class TypeExtensions
{
    public static string GloballyQualifiedName(this Type self)
    {
        return $"global::{self}";
    }
}
