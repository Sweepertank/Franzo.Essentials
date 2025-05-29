using System.Text;
using Franzo.Essentials.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class TypeSymbolExtensions
{
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

    public static bool IsAssignableTo(
        this ITypeSymbol self,
        ITypeSymbol other,
        CSharpCompilation compilation)
    {
        var conversion = compilation.ClassifyConversion(self, other);
        return conversion.IsImplicit && !conversion.IsUserDefined;
    }

    public static bool IsCompilerGeneratedAttributeType(this ITypeSymbol self)
    {
        return self.ToDisplayString().StartsWith("System.Runtime.CompilerServices");
    }

    public static string MemberifiedName(this ITypeSymbol self, TypeEmissionContext cxt)
    {
        return cxt.MainContext.GetMemberifiedName(self);
    }

    public static string MemberifiedNameCore(this ITypeSymbol self, Context cxt)
    {
        var sb = new StringBuilder();
        if (self.ContainingType is not null)
        {
            sb.Append(self.ContainingType.ToMangledTypeQualifiedWithTypeParametersName(cxt));
            sb.Append("_");
        }

        sb.Append(self.Name.WithoutInterfaceI());

        if (self is INamedTypeSymbol namedSelf && namedSelf.TypeArguments.Length > 0)
        {
            sb.Append("__");

            foreach ((var typeArgument, var last) in namedSelf.TypeArguments.WithLastFlag())
            {
                sb.Append(
                    typeArgument.ToMangledTypeQualifiedWithTypeParametersName(cxt));

                if (!last)
                {
                    sb.Append("_");
                }
            }

            sb.Append("__");
        }

        return sb.ToString();
    }

    public static string ToMangledTypeQualifiedWithTypeParametersName(
        this ITypeSymbol self,
        Context cxt)
    {
        return cxt.GetMangledTypeQualifiedWithTypeParametersName(self);
    }

    public static string ToMangledTypeQualifiedWithTypeParametersNameCore(
        this ITypeSymbol self)
    {
        return self.ToDisplayString(TypeQualifiedWithTypeParametersFormat).MangleSymbolName();
    }
}
