using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal abstract class InternalFeatureSymbol : InternalMemberSymbol
{
    public List<InternalTypeSymbol> TypesDeclaringShadowingFeatures = [];
    public bool HasOverrideAttribute = false;

    public InternalFeatureSymbol(ISymbol roslynSymbol, InternalAnalysisContext context)
        : base(roslynSymbol, context)
    {
    }
}
