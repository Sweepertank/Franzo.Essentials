using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Franzo.Essentials.Roslyn;

public static class SymbolExtensions
{
    private static SymbolDisplayFormat FullyQualifiedWithNullableReferenceTypeAnnotationsFormat =
        SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    // Adapted from CSharpShortErrorMessageFormat
    // in https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/SymbolDisplay/SymbolDisplayFormat.cs
    private static SymbolDisplayFormat SimpleFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
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

    static SymbolExtensions()
    {
        var formatConstructor = typeof(SymbolDisplayFormat)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single();
    }

    public static bool CorrectEquals(this ISymbol? self, ISymbol? other)
    {
        return SymbolEqualityComparer.Default.Equals(self, other);
    }

    public static IEnumerable<INamedTypeSymbol> ContainingTypes(this ISymbol self)
    {
        var type = self.ContainingType;
        while (type is not null)
        {
            yield return type;
            type = type.ContainingType;
        }
    }

    public static IEnumerable<ISymbol> SelfAndContainingTypes(this ISymbol self)
    {
        yield return self;
        foreach (var type in self.ContainingTypes())
        {
            yield return type;
        }
    }

    public static bool AreSelfAndContainingTypesPartiallyDeclared(this ISymbol self)
    {
        return self.SelfAndContainingTypes().All(s => s.IsPartiallyDeclared());
    }

    public static bool IsPartiallyDeclared(this ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax() is not MemberDeclarationSyntax memberDeclaration)
            {
                return false;
            }
            else if (!memberDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                return false;
            }
        }

        return true;
    }

    public static string ToFullyQualifiedDisplayString(this ISymbol self)
    {
        return self.ToDisplayString(FullyQualifiedWithNullableReferenceTypeAnnotationsFormat);
    }

    public static string ToCSharpShortErrorMessageString(this ISymbol self)
    {
        return self.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);
    }

    public static string ToSimpleDisplayString(this ISymbol self)
    {
        return self.ToDisplayString(SimpleFormat);
    }

    public static void WriteVerbatimizedName(this ISymbol self, TextWriter writer)
    {
        writer.Write("@");
        writer.Write(self.Name);
    }

    // @todo: this should return T eventually
    public static IEnumerable<AttributeData> GetAttributes<T>(this ISymbol self) where T : Attribute
    {
        return self.GetAttributes().Where(
            a => a.AttributeClass?.ToDisplayString() == typeof(T).FullName);
    }

    public static IEnumerable<AttributeData> GetAttributes(this ISymbol self, Type attributeType)
    {
        return self.GetAttributes().Where(
            a => a.AttributeClass?.ToDisplayString() == attributeType.FullName);
    }

    public static IEnumerable<AttributeData> GetGenericAttributes(
        this ISymbol self,
        Type attributeTypeGenericDefinition)
    {
        return self.GetAttributes().Where(
            a => a.AttributeClass is not null
                && Regex.IsMatch(
                    a.AttributeClass.ToDisplayString(),
                    $"^{attributeTypeGenericDefinition.FullUngenericizedName()}<.*>$"));
    }

    public static bool HasAttribute<T>(this ISymbol self) where T : Attribute
    {
        return self.GetAttributes<T>().Any();
    }

    public static bool HasGenericAttribute(
        this ISymbol self,
        Type attributeGenericTypeDefinition)
    {
        return self.GetGenericAttributes(attributeGenericTypeDefinition).Any();
    }
}
