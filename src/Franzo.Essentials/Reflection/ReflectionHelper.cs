using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class ReflectionHelper
{
    public const BindingFlags PublicInstanceBindingFlags =
        BindingFlags.Public | BindingFlags.Instance;

    public const BindingFlags PublicInstanceIgnoreCaseBindingFlags =
        PublicInstanceBindingFlags | BindingFlags.IgnoreCase;

    public const BindingFlags PublicInstanceStaticDeclaredOnlyBindingFlags =
        PublicInstanceBindingFlags | BindingFlags.Static | BindingFlags.DeclaredOnly;
}
