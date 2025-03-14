using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.Roslyn;

public static class NamedTypeSymbolExtensions
{
    public static bool IsConstructedGenericType(this INamedTypeSymbol self)
    {
        return self.IsGenericType && !self.CorrectEquals(self.ConstructedFrom);
    }

    public static bool IsConstructedGenericTypeOrWithinConstructedGenericType(this INamedTypeSymbol self)
    {
        return self.IsConstructedGenericType()
            || (self.ContainingType is not null && self.ContainingType.IsConstructedGenericTypeOrWithinConstructedGenericType());
    }

    public static IEnumerable<INamedTypeSymbol> SelfAndContainingTypes(this INamedTypeSymbol self)
    {
        return ((ISymbol)self).SelfAndContainingTypes().Cast<INamedTypeSymbol>();
    }
}
