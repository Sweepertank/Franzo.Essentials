namespace Franzo.Essentials.ComponentModel;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class DefaultValueFieldAttribute : Attribute
{
    public string FieldName { get; }

    public DefaultValueFieldAttribute(string fieldName)
    {
        FieldName = fieldName;
    }
}
