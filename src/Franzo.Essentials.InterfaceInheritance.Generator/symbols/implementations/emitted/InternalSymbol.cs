using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal abstract class InternalSymbol : IInternalSymbol
{
    public readonly ISymbol RoslynSymbol;

    public InternalSymbol(ISymbol roslynSymbol)
    {
        RoslynSymbol = roslynSymbol;
    }
}
