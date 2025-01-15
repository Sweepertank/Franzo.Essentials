namespace Franzo.Essentials;

public class NqString
{
    public string Value { get; }

    public NqString(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}
