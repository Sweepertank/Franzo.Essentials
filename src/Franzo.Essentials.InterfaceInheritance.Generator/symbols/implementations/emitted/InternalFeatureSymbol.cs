using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal abstract class InternalFeatureSymbol : InternalMemberSymbol
{
    public List<InternalTypeSymbol> TypesDeclaringShadowingFeatures = new();
    public bool HasOverrideAttribute = false;

    public InternalFeatureSymbol(ISymbol roslynSymbol) : base(roslynSymbol)
    {
    }
}
