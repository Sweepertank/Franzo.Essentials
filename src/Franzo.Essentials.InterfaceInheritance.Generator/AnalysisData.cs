using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class AnalysisData
{
    public readonly List<InternalTypeSymbol> RootTypesToEmit = new();
    public readonly List<InternalTypeSymbol> TypesCreatedDuringTypeInitializationPhase1 = new();
    public readonly List<InternalTypeSymbol> TypesCreatedDuringTypeInitializationPhase2 = new();
    public readonly Dictionary<INamedTypeSymbol, InternalTypeSymbol> TypesByRoslynType = new(
        SymbolEqualityComparer.Default);

    internal IEnumerable<InternalTypeSymbol> Types
    {
        get => TypesCreatedDuringTypeInitializationPhase1.Concat(
            TypesCreatedDuringTypeInitializationPhase2);
    }
}
