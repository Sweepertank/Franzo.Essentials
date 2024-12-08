namespace Franzo.Essentials.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public class InsertMethodAttribute : Attribute
{
    public string MethodName { get; }

    public InsertMethodAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
