namespace Franzo.Essentials.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public class RemoveMethodAttribute : Attribute
{
    public string MethodName { get; }

    public RemoveMethodAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
