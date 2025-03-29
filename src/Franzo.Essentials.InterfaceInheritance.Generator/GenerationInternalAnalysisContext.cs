using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class GenerationInternalAnalysisContext : InternalAnalysisContext
{
    private readonly object _diagnosticReportingLock = new();

    public Context InnerContext { get; }

    public override CancellationToken CancellationToken
    {
        get => InnerContext.SourceProductionContext.CancellationToken;
    }

    public override CSharpCompilation Compilation
    {
        get => InnerContext.Compilation;
    }

    public override ImmutableHashSet<INamedTypeSymbol> PossiblyRelevantTopLevelRoslynTypes
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
        lock (_diagnosticReportingLock)
        {
            InnerContext.SourceProductionContext.ReportDiagnostic(diagnostic);
        }
    }
}
