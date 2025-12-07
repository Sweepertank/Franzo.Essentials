using Mono.Cecil;

namespace Franzo.Essentials.MSBuild;

internal static class MethodDefinitionExtensions
{
    public static bool IsExplicitInterfaceMethodImplementation(this MethodDefinition self)
    {
        return self.IsPrivate
            && self.IsFinal
            && self.IsHideBySig
            && self.IsNewSlot
            && self.IsVirtual;
    }
}
