using OneOf;

namespace Franzo.Essentials.Reflection;

public class TextFilePositionOrOffset : OneOfBase<TextFilePosition, int>
{
    public TextFilePositionOrOffset(OneOf<TextFilePosition, int> _) : base(_)
    {
    }

    public static implicit operator TextFilePositionOrOffset(TextFilePosition _)
        => new TextFilePositionOrOffset(_);

    public static explicit operator TextFilePosition(TextFilePositionOrOffset _)
        => _.AsT0;

    public static implicit operator TextFilePositionOrOffset(int _)
        => new TextFilePositionOrOffset(_);

    public static explicit operator int(TextFilePositionOrOffset _)
        => _.AsT1;
}
