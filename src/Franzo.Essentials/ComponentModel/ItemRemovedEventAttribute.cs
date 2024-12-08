namespace Franzo.Essentials.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public class ItemRemovedEventAttribute : Attribute
{
    public string EventName { get; }

    public ItemRemovedEventAttribute(string eventName)
    {
        EventName = eventName;
    }
}
