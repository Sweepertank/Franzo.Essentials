using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class MethodSymbolExtensions
{
    public static bool CanBeCalledWithZeroArgumentsInSource(this IMethodSymbol self)
    {
        return self.Parameters.All(p => p.HasExplicitDefaultValue);
    }
}
