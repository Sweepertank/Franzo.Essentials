using System.Collections.Concurrent;
using System.Collections.Immutable;
using Franzo.Essentials.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class Context
{
    private readonly ConcurrentDictionary<ISymbol, string> _fullyQualifiedWithNullableReferenceTypeAnnotationsDisplayStrings =
        new(JohnLockeEnvironment.ProcessorCount, 2048, SymbolEqualityComparer.IncludeNullability);

    //private readonly ConcurrentDictionary<INamedTypeSymbol, string> _memberifiedNames =
    //    new(JohnLockeEnvironment.ProcessorCount, 1024, SymbolEqualityComparer.IncludeNullability);

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

    public string GetFullyQualifiedWithNullableReferenceTypeAnnotationsDisplayString(
        ISymbol roslynSymbol)
    {
        return _fullyQualifiedWithNullableReferenceTypeAnnotationsDisplayStrings.GetOrAdd(
            roslynSymbol,
            s => s.ToFullyQualifiedWithNullableReferenceTypeAnnotationsDisplayString());
    }

    /*public string GetMemberifiedName(INamedTypeSymbol roslynSymbol)
    {
        return _memberifiedNames.GetOrAdd(
            roslynSymbol,
            s => s.MemberifiedNameCore());
    }*/
}
