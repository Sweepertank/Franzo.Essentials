using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

[Generator]
public partial class InterfaceInheritanceGenerator : IIncrementalGenerator
{
    internal const string InterfaceDataClassName = "Data_";
    internal const string GeneratedInterfaceDataPropertyName = "__Data_Prop";
    internal const string GeneratedConstructorAccessorName = "__Construct";
    internal const string DataString = "Data";

    public void Initialize(IncrementalGeneratorInitializationContext cxt)
    {
        // @todo: use cxt.SyntaxProvider.ForAttributeWithMetadataName
        var types = cxt.SyntaxProvider
            .CreateSyntaxProvider(
                (node, _) => IsNodePossiblyRelevant(node),
                (cxt, _) => GetNodeBoundTypeIfTopLevel(cxt))
            .Where(t => t is not null);

        cxt.RegisterSourceOutput(
            cxt.CompilationProvider.Combine(types.Collect()),
            Execute!);
    }

    private static bool IsNodePossiblyRelevant(SyntaxNode node)
    {
        return node is TypeDeclarationSyntax;
    }

    private static INamedTypeSymbol? GetNodeBoundTypeIfTopLevel(GeneratorSyntaxContext cxt)
    {
        var type = cxt.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)cxt.Node);
        if (type is null)
        {
            return null;
        }

        return type.ContainingType is null ? type : null;
    }

    private static void Execute(
        SourceProductionContext sourceProductionContext,
        (Compilation, ImmutableArray<INamedTypeSymbol>) source)
    {
        var cxt = new Context(
            (CSharpCompilation)source.Item1,
            source.Item2.ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default),
            sourceProductionContext);

        Analyze(new GenerationInternalAnalysisContext(cxt));
        Emit(cxt);
    }
}
