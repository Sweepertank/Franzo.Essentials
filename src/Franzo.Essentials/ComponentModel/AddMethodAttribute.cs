namespace Franzo.Essentials.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public class AddMethodAttribute : Attribute
{
    public string MethodName { get; }

    public AddMethodAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
