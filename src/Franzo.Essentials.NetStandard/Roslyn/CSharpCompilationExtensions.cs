using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.Roslyn;

public static class CSharpCompilationExtensions
{
    public static IEnumerable<INamedTypeSymbol> GetForwardedTypes(this CSharpCompilation self)
    {
        // https://github.com/dotnet/roslyn/issues/6138
        var stack = new Stack<INamespaceSymbol>();
        stack.Push(self.GlobalNamespace);

        while (stack.Count > 0)
        {
            var @namespace = stack.Pop();
            foreach (var member in @namespace.GetMembers())
            {
                if (member is INamespaceSymbol memberAsNamespace)
                {
                    stack.Push(memberAsNamespace);
                }
                else if (member is INamedTypeSymbol memberAsNamedTypeSymbol)
                {
                    yield return memberAsNamedTypeSymbol;
                }
            }
        }
    }
}
