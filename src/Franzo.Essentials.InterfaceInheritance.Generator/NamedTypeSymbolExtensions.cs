using Microsoft.CodeAnalysis;

namespace Franzo.Essentials;

internal static class NamedTypeSymbolExtensions
{
    public static bool IsCompilerGeneratedAttributeType(this INamedTypeSymbol self)
    {
        return self.ToDisplayString()
            is "System.Runtime.CompilerServices.NullableContextAttribute"
            or "System.Runtime.CompilerServices.NullableAttribute";
    }
}
