using System.Diagnostics;
using Franzo.Essentials.InterfaceInheritance;
using Mono.Cecil;

namespace Franzo.Essentials.MSBuild;

internal static class CustomAttributeProviderExtensions
{
    public static bool HasInterfaceInheritanceDevirtualizeAttribute(this ICustomAttributeProvider self)
    {
        return self.CustomAttributes.Any(
#pragma warning disable CS0618
            a => a.AttributeType.FullName == typeof(InterfaceInheritanceDevirtualizeAttribute).FullName);
#pragma warning restore CS0618
    }

    public static bool HasDebuggerBrowsableAttribute(this ICustomAttributeProvider self)
    {
        return self.CustomAttributes.Any(
            a => a.AttributeType.FullName == typeof(DebuggerBrowsableAttribute).FullName);
    }
}
