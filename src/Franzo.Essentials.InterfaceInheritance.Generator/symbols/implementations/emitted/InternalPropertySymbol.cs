using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class InternalPropertySymbol :
    InternalFeatureSymbol,
    IInternalPropertySymbol
{
    public new IPropertySymbol RoslynSymbol
    {
        get => (IPropertySymbol)base.RoslynSymbol;
    }

    public InternalPropertySymbol(IPropertySymbol roslynSymbol, InternalAnalysisContext context)
        : base(roslynSymbol, context)
    {
    }
}
