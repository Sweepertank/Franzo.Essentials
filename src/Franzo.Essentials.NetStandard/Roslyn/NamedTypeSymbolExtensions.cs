using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.Roslyn;

public static class NamedTypeSymbolExtensions
{
    public static bool IsConstructedGenericType(this INamedTypeSymbol self)
    {
        return self.IsGenericType && !self.CorrectEquals(self.ConstructedFrom);
    }
}
