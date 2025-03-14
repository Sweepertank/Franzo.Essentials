using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal abstract class InternalSymbol : IInternalSymbol
{
    public readonly ISymbol RoslynSymbol;
    public readonly InternalAnalysisContext Context;

    public InternalSymbol(ISymbol roslynSymbol, InternalAnalysisContext cxt)
    {
        RoslynSymbol = roslynSymbol;
        Context = cxt;
    }
}
