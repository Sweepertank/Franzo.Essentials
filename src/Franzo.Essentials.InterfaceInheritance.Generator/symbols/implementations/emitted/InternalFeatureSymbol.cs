using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal abstract class InternalFeatureSymbol : InternalMemberSymbol
{
    public List<InternalTypeSymbol> TypesDeclaringShadowingFeatures = new();
    public readonly object TypesDeclaringShadowingFeaturesLock = new();
    public bool HasOverrideAttribute = false;

    public InternalFeatureSymbol(ISymbol roslynSymbol, InternalAnalysisContext cxt)
        : base(roslynSymbol, cxt)
    {
    }
}
