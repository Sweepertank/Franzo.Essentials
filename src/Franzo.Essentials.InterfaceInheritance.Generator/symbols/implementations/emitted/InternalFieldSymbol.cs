using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class InternalFieldSymbol :
    InternalFeatureSymbol,
    IInternalPropertySymbol
{
    public new IFieldSymbol RoslynSymbol
    {
        get => (IFieldSymbol)base.RoslynSymbol;
    }

    public InternalFieldSymbol(IFieldSymbol roslynSymbol) : base(roslynSymbol)
    {
    }
}
