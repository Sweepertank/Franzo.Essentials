using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class RefKindExtensions
{
    public static string? ToCSharpString(this RefKind self)
    {
        return self switch
        {
            RefKind.In => "in",
            RefKind.Out => "out",
            RefKind.Ref => "ref",
            RefKind.None => null,
            _ => throw new ShouldNeverBeThrownException()
        };
    }
}
