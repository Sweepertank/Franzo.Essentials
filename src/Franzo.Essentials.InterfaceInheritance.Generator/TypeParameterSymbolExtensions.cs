using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class TypeParameterSymbolExtensions
{
    public static bool HasConstraints(this ITypeParameterSymbol self)
    {
        return self.HasReferenceTypeConstraint
            || self.HasValueTypeConstraint
            || self.HasUnmanagedTypeConstraint
            || self.HasNotNullConstraint
            || self.ConstraintTypes.Length > 0
            || self.HasConstructorConstraint;
    }
}
