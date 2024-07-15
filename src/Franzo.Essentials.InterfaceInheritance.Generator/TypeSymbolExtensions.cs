using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class TypeSymbolExtensions
{
    public static bool IsAssignableTo(
        this ITypeSymbol self,
        ITypeSymbol other,
        CSharpCompilation compilation)
    {
        var conversion = compilation.ClassifyConversion(self, other);
        return conversion.IsImplicit && !conversion.IsUserDefined;
    }
}
