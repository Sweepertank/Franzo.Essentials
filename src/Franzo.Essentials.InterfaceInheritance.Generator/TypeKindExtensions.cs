using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class TypeKindExtensions
{
    public static bool IsClassOrStruct(this TypeKind self)
    {
        return self is TypeKind.Class or TypeKind.Struct;
    }
}
