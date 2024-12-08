using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class NetStandardReflectionHelper
{
    public const BindingFlags DefaultBindingFlags =
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
}
