using System.Diagnostics;

namespace Franzo.Essentials;

public readonly record struct TextFileSpan : IComparable<TextFileSpan>
{
    public static readonly TextFileSpan Empty = new(
        TextFilePosition.Start,
        TextFilePosition.Start,
        0);

    public TextFilePosition Start { get; }
    public TextFilePosition End { get; }
    public int Width { get; }

    public TextFileSpan(
        TextFilePosition start,
        TextFilePosition end,
        int width = -1)
    {
        Start = start;
        End = end;
        Width = width;
    }

    public bool Contains(TextFilePosition position)
    {
        return position >= Start && position < End;
    }

    public bool Equals(TextFileSpan other)
    {
        return AreEqual(this, other);
    }

    public static bool AreEqual(TextFileSpan? a, TextFileSpan? b)
    {
        if (a is null) return b is null;
        if (b is null) return a is null;

        if (a.Value.Width >= 0 && b.Value.Width >= 0 && a.Value.Width != b.Value.Width)
        {
            return false;
        }

        return a.Value.Start == b.Value.Start && a.Value.End == b.Value.End;
    }

    public override int GetHashCode()
    {
        return (Start, End).GetHashCode();
    }

    public string ToString(bool startPositionOnly = false)
    {
        return startPositionOnly
            ? Start.ToString()
            : $"[{Start} - {End}]";
    }

    public override string ToString()
    {
        return ToString(false);
    }

    public int CompareTo(TextFileSpan other)
    {
        return Compare(this, other);
    }

    public static int Compare(TextFileSpan? a, TextFileSpan? b)
    {
        if (a is null && b is not null) return -1;
        if (b is null && a is not null) return 1;
        if (a is null && b is null) return 0;

        Debug.Assert(a is not null);
        Debug.Assert(b is not null);

        var startPositionComparison = a.Value.Start.CompareTo(b.Value.Start);
        if (startPositionComparison != 0)
        {
            return startPositionComparison;
        }

        var endPositionComparison = a.Value.End.CompareTo(b.Value.End);
        if (endPositionComparison != 0)
        {
            return endPositionComparison;
        }

        if (a.Value.Width >= 0 && b.Value.Width >= 0)
        {
            var widthComparison = a.Value.Width.CompareTo(b.Value.Width);
            if (widthComparison != 0)
            {
                return widthComparison;
            }
        }

        return 0;
    }

    public static bool operator <(TextFileSpan a, TextFileSpan b)
    {
        return Compare(a, b) < 0;
    }

    public static bool operator <=(TextFileSpan a, TextFileSpan b)
    {
        return Compare(a, b) <= 0;
    }

    public static bool operator >(TextFileSpan a, TextFileSpan b)
    {
        return Compare(a, b) > 0;
    }

    public static bool operator >=(TextFileSpan a, TextFileSpan b)
    {
        return Compare(a, b) >= 0;
    }
}
