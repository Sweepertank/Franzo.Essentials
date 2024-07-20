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
    internal const string GeneratedConstructorPropertyName = "__Constructor";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var types = context.SyntaxProvider
            .CreateSyntaxProvider<INamedTypeSymbol?>(
                (node, _) => IsNodePossiblyRelevant(node),
                (cxt, _) => GetNodeBoundTypeIfTopLevel(cxt))
            .Where(t => t is not null);

        // @todo: use context.SyntaxProvider.ForAttributeWithMetadataName

        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(types.Collect()),
            Execute!);
    }

    private static void SuppressVirtualCallsPostfix(object __instance, ref bool __result)
    {
        //var syntax = _syntaxGetter.Invoke(__instance);
        Console.WriteLine("HOY");
        //Console.WriteLine(syntax.ToFullString());
    }

    private static bool IsNodePossiblyRelevant(SyntaxNode node)
    {
        return node is TypeDeclarationSyntax;
    }

    private static INamedTypeSymbol? GetNodeBoundTypeIfTopLevel(GeneratorSyntaxContext context)
    {
        var type = context.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)context.Node);
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
        var context = new Context(
            (CSharpCompilation)source.Item1,
            source.Item2,
            sourceProductionContext);

        Analyze(new GenerationInternalAnalysisContext(context));
        Emit(context);
    }
}
