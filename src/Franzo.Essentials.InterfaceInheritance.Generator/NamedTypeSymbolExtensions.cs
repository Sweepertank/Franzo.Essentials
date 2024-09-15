using System.Text;
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
        return self.ToDisplayString()
            is "System.Runtime.CompilerServices.NullableContextAttribute"
            or "System.Runtime.CompilerServices.NullableAttribute";
    }

    public static string MemberifiedName(this INamedTypeSymbol self)
    {
        string Mangle(string str)
        {
            return str.Replace('.', '_')
                .Replace("<", "__")
                .Replace(">", "__")
                .Replace(", ", "_");
        }

        var s = self.Name.WithoutInterfaceI();

        if (self.TypeArguments.Length > 0)
        {
            var sb = new StringBuilder();
            sb.Append("__");

            foreach ((var i, var typeArgument) in self.TypeArguments.IndicesAndItems())
            {
                sb.Append(
                    Mangle(typeArgument.ToDisplayString(TypeQualifiedWithTypeParametersFormat)));

                if (i != self.TypeArguments.Length - 1)
                {
                    sb.Append("_");
                }
            }

            sb.Append("__");

            s += sb.ToString();
        }

        if (self.ContainingType is not null)
        {
            s = Mangle(self.ContainingType.ToDisplayString(TypeQualifiedWithTypeParametersFormat))
                + "_"
                + s;
        }

        return s;
    }
}
