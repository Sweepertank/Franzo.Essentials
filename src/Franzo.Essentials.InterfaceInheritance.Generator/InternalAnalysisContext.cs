using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal abstract class InternalAnalysisContext
{
    public abstract CSharpCompilation Compilation { get; }
    public abstract ImmutableHashSet<INamedTypeSymbol> PossiblyRelevantTopLevelRoslynTypes { get; }
    public abstract AnalysisData Data { get; }

    public abstract void ReportDiagnostic(Diagnostic diagnostic);
}
