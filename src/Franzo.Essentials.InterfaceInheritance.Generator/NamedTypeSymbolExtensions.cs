using System.Text;
using Franzo.Essentials.Collections;
using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class NamedTypeSymbolExtensions
{
    // Adapted from CSharpShortErrorMessageFormat
    // in https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/SymbolDisplay/SymbolDisplayFormat.cs
    public static readonly SymbolDisplayFormat TypeQualifiedFormat = new(
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

    public static readonly SymbolDisplayFormat TypeQualifiedWithTypeParametersFormat =
        TypeQualifiedFormat.WithGenericsOptions(
            SymbolDisplayGenericsOptions.IncludeTypeParameters);

    public static bool IsCompilerGeneratedAttributeType(this INamedTypeSymbol self)
    {
        return self.ToDisplayString().StartsWith("System.Runtime.CompilerServices");
    }

    public static string MemberifiedName(this INamedTypeSymbol self)
    {
        var str = self.Name.WithoutInterfaceI();

        if (self.TypeArguments.Length > 0)
        {
            var sb = new StringBuilder();
            sb.Append("__");

            foreach ((var typeArgument, var last) in self.TypeArguments.WithLastFlag())
            {
                sb.Append(
                    typeArgument.ToDisplayString(TypeQualifiedWithTypeParametersFormat).MangleSymbolName());

                if (!last)
                {
                    sb.Append("_");
                }
            }

            sb.Append("__");

            str += sb.ToString();
        }

        if (self.ContainingType is not null)
        {
            str = self.ContainingType.ToDisplayString(TypeQualifiedWithTypeParametersFormat).MangleSymbolName()
                + "_"
                + str;
        }

        return str;
    }
}
