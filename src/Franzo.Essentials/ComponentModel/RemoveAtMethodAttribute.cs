namespace Franzo.Essentials.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public class RemoveAtMethodAttribute : Attribute
{
    public string MethodName { get; }

    public RemoveAtMethodAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
