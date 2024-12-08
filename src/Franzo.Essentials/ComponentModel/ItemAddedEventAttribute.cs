namespace Franzo.Essentials.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public class ItemAddedEventAttribute : Attribute
{
    public string EventName { get; }

    public ItemAddedEventAttribute(string eventName)
    {
        EventName = eventName;
    }
}
