using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class StringExtensions
{
    public static bool TryBindToType(
        this string str,
        INamedTypeSymbol scope,
        CSharpCompilation compilation,
        out ITypeSymbol? result)
    {
        Debug.Assert(scope.DeclaringSyntaxReferences.Length > 0);

        var scopeReference = scope.DeclaringSyntaxReferences[0];
        var scopeSyntax = (TypeDeclarationSyntax)scopeReference.GetSyntax();
        var scopeSemantics = compilation.GetSemanticModel(scopeSyntax.SyntaxTree);

        var typeInfo = scopeSemantics.GetSpeculativeTypeInfo(
            scopeSyntax.OpenBraceToken.Span.Start,
            SyntaxFactory.ParseTypeName(str),
            SpeculativeBindingOption.BindAsTypeOrNamespace);

        if (typeInfo.Type is null)
        {
            result = null;
            return false;
        }

        result = typeInfo.Type;
        return true;
    }

    public static string WithoutInterfaceI(this string self)
    {
        if (self.StartsWith("I") && self.Length > 1 && self[1].IsUpper() && self.Length != 2)
        {
            return self.Substring(1);
        }

        return self;
    }

    public static string MangleSymbolName(this string str)
    {
        return str.Replace('.', '_')
            .Replace("<", "__")
            .Replace(">", "__")
            .Replace(", ", "_");
    }
}
