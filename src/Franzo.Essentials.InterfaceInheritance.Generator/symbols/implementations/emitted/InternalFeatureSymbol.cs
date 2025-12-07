using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal abstract class InternalFeatureSymbol : InternalMemberSymbol
{
    public List<InternalTypeSymbol> TypesDeclaringShadowingFeatures = new();
    public readonly object TypesDeclaringShadowingFeaturesLock = new();
    public readonly List<InternalFeatureSymbol> ShadowedFeatures = new();
    public bool HasOverrideAttribute = false;
    public List<InternalFeatureSymbol> ImplicitlyImplementableShadowedPublicAbstractFeatures = new();

    public IEnumerable<InternalFeatureSymbol> MaskedFeatures
    {
        get => new[] { this }.Concat(ShadowedFeatures);
    }

    public IEnumerable<InternalFeatureSymbol> SelfAndShadowedImplicitlyImplementablePublicAbstractFeatures
    {
        get => new[] { this }.Concat(ImplicitlyImplementableShadowedPublicAbstractFeatures);
    }

    public InternalFeatureSymbol(ISymbol roslynSymbol, InternalAnalysisContext cxt)
        : base(roslynSymbol, cxt)
    {
    }
}
