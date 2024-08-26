using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class NamedTypeSymbolExtensions
{
    // Adapted from CSharpShortErrorMessageFormat
    // in https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/SymbolDisplay/SymbolDisplayFormat.cs
    public static SymbolDisplayFormat TypeQualifiedFormat { get; } = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
        propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.None,
        memberOptions:
            SymbolDisplayMemberOptions.IncludeParameters |
            SymbolDisplayMemberOptions.IncludeContainingType |
            SymbolDisplayMemberOptions.IncludeExplicitInterface,
        parameterOptions:
            SymbolDisplayParameterOptions.IncludeParamsRefOut |
            SymbolDisplayParameterOptions.IncludeType,
        miscellaneousOptions:
            SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
            SymbolDisplayMiscellaneousOptions.UseAsterisksInMultiDimensionalArrays |
            SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    public static bool IsCompilerGeneratedAttributeType(this INamedTypeSymbol self)
    {
        return self.ToDisplayString()
            is "System.Runtime.CompilerServices.NullableContextAttribute"
            or "System.Runtime.CompilerServices.NullableAttribute";
    }

    public static string MemberifiedName(this INamedTypeSymbol self)
    {
        var s = self.ToDisplayString(TypeQualifiedFormat);
        var indexOfLastDot = s.LastIndexOf('.');

        var simpleNameStartIndex = 0;
        if (indexOfLastDot >= 0)
        {
            simpleNameStartIndex = indexOfLastDot + 1;
        }

        s = s.Replace('.', '_');

        if (simpleNameStartIndex < s.Length)
        {
            return s.Substring(0, simpleNameStartIndex)
                + s.Substring(simpleNameStartIndex, s.Length - simpleNameStartIndex).WithoutInterfaceI();
        }

        return s;
    }
}
