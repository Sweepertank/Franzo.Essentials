namespace Franzo.Essentials.InterfaceInheritance;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Method, AllowMultiple = true)]
[Obsolete($"{nameof(InterfaceInheritanceDevirtualizeAttribute)} should never be used directly.")]
public class InterfaceInheritanceDevirtualizeAttribute : Attribute
{
}
