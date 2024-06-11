using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class InternalEventSymbol :
    InternalFeatureSymbol,
    IInternalEventSymbol
{
    public new IEventSymbol RoslynSymbol
    {
        get => (IEventSymbol)base.RoslynSymbol;
    }

    public InternalEventSymbol(IEventSymbol roslynSymbol) : base(roslynSymbol)
    {
    }
}
