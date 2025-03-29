using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class AnalysisData
{
    public readonly List<InternalTypeSymbol> RootTypesToEmit = new(1024);
    public readonly object RootTypesToEmitLock = new();
    public readonly List<InternalTypeSymbol> TypesCreatedDuringTypeInitializationPhase1 = new(1024);
    public readonly List<InternalTypeSymbol> TypesCreatedDuringTypeInitializationPhase2 = new(1024);
    public readonly Dictionary<INamedTypeSymbol, InternalTypeSymbol> TypesByRoslynType = new(
        2048,
        SymbolEqualityComparer.Default);
    public readonly object TypeCreationLock = new();

    internal IEnumerable<InternalTypeSymbol> Types
    {
        get => TypesCreatedDuringTypeInitializationPhase1.Concat(
            TypesCreatedDuringTypeInitializationPhase2);
    }
}
