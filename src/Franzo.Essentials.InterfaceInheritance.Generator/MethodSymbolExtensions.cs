using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class MethodSymbolExtensions
{
    public static bool CanBeCalledWithZeroArgumentsInSource(this IMethodSymbol self)
    {
        return self.Parameters.All(p => p.HasExplicitDefaultValue);
    }
    /*public static bool AccessorIsInheritable(
        this IMethodSymbol self)
    {
    }*/

    /*public static void WriteUnsafeAccessorName(this IMethodSymbol self, TextWriter writer)
    {
        writer.Write("access_");
        writer.Write(self.Name);
    }*/
}
