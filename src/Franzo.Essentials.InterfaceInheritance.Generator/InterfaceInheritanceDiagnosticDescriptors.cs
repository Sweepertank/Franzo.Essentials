using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor Dummy = new(
        "II001",
        "Dummy",
        "dummy",
        $"Franzo.Essentials.InterfaceInheritance",
        DiagnosticSeverity.Error,
        true);
}
