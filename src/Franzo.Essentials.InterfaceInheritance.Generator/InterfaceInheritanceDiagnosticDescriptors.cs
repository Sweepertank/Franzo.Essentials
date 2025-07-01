using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor InterfaceDataClassMustBeClass = new(
        "II001",
        "Interface data class must be class",
        "Interface data class must be a class, not an interface or struct",
        $"Franzo.Essentials.InterfaceInheritance",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor InterfaceDataClassCannotHaveTypeParameters = new(
        "II002",
        "Interface data class cannot have type parameters",
        "An interface data class cannot have type parameters",
        $"Franzo.Essentials.InterfaceInheritance",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor InterfaceDataClassMustBePublic = new(
        "II003",
        "Interface data class must be public",
        "An interface data class must be public",
        $"Franzo.Essentials.InterfaceInheritance",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor InterfaceDataClassMustDeclareExactlyOnePublicConstructor = new(
        "II004",
        "Interface data class must declare exactly one public constructor",
        "An interface data class must declare exactly one public constructor",
        $"Franzo.Essentials.InterfaceInheritance",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor NonDatafulInterfaceCannotImplementDatafulInterface = new(
        "II005",
        "Non-dataful interface cannot implement dataful interface",
        "Non-dataful interface '{0}' cannot implement dataful interface '{1}'",
        $"Franzo.Essentials.InterfaceInheritance",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor TypeMustDeclareConstructor = new(
        "II006",
        "Type must declare constructor",
        "Type '{0}' must declare a constructor because it implements a dataful interface '{1}' whose data class constructor requires arguments",
        $"Franzo.Essentials.InterfaceInheritance",
        DiagnosticSeverity.Error,
        true);
}
