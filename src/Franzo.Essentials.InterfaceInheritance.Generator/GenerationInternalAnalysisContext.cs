using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class GenerationInternalAnalysisContext : InternalAnalysisContext
{
    public Context InnerContext { get; }

    public override CSharpCompilation Compilation
    {
        get => InnerContext.Compilation;
    }

    public override ImmutableArray<INamedTypeSymbol> PossiblyRelevantTopLevelRoslynTypes
    {
        get => InnerContext.PossiblyRelevantTopLevelRoslynTypes;
    }

    public override AnalysisData Data
    {
        get => InnerContext.AnalysisData;
    }

    public GenerationInternalAnalysisContext(Context innerContext)
    {
        InnerContext = innerContext;
    }

    public override void ReportDiagnostic(Diagnostic diagnostic)
    {
        InnerContext.SourceProductionContext.ReportDiagnostic(diagnostic);
    }
}
