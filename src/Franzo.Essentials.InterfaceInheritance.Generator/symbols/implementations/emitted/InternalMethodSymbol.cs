using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class InternalMethodSymbol :
    InternalFeatureSymbol,
    IInternalMethodSymbol
{
    public new IMethodSymbol RoslynSymbol
    {
        get => (IMethodSymbol)base.RoslynSymbol;
    }

    public InternalMethodSymbol(IMethodSymbol roslynSymbol) : base(roslynSymbol)
    {
    }
}
