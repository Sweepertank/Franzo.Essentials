using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class MethodKindExtensions
{
    public static bool IsAccessor(this MethodKind self)
    {
        return self is MethodKind.PropertyGet
            or MethodKind.PropertySet
            or MethodKind.EventAdd
            or MethodKind.EventRemove;
    }
}
