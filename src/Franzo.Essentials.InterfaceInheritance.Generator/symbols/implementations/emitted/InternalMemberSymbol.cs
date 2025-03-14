using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal abstract class InternalMemberSymbol : InternalSymbol, IInternalMemberSymbol
{
    public InternalMemberSymbol(ISymbol roslynSymbol, InternalAnalysisContext cxt)
        : base(roslynSymbol, cxt)
    {
    }
}
