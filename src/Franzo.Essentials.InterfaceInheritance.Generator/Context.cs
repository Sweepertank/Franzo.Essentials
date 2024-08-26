using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class Context
{
    public readonly CSharpCompilation Compilation;
    public readonly ImmutableHashSet<INamedTypeSymbol> PossiblyRelevantTopLevelRoslynTypes;
    public readonly AnalysisData AnalysisData = new();
    public SourceProductionContext SourceProductionContext;

    public Context(
        CSharpCompilation compilation,
        ImmutableHashSet<INamedTypeSymbol> possiblyRelevantTopLevelRoslynTypes,
        SourceProductionContext sourceProductionContext)
    {
        Compilation = compilation;
        PossiblyRelevantTopLevelRoslynTypes = possiblyRelevantTopLevelRoslynTypes;
        SourceProductionContext = sourceProductionContext;
    }
}
