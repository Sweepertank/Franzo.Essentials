using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class FieldSymbolExtensions
{
    public static void WriteUnsafeAccessorName(
        this IFieldSymbol self,
        TextWriter writer)
    {
        writer.Write("access_");
        writer.Write(self.Name);
    }
}
