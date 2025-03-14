using System.CodeDom.Compiler;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class TypeEmissionContext
{
    public Context MainContext { get; }
    public IndentedTextWriter Writer { get; }
    public Stack<InternalTypeSymbol> TypeStack { get; }

    public TypeEmissionContext(Context mainContext, IndentedTextWriter writer)
    {
        MainContext = mainContext;
        Writer = writer;
        TypeStack = new Stack<InternalTypeSymbol>();
    }
}
