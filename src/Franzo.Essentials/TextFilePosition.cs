using System.Diagnostics;

namespace Franzo.Essentials;

public readonly record struct TextFilePosition : IComparable<TextFilePosition>
{
    public static readonly TextFilePosition Start = new(1, 1, 0);

    public int Line { get; }
    public int Column { get; }
    public int Offset { get; }

    public TextFilePosition(int line, int column, int offset = -1)
    {
        Line = line;
        Column = column;
        Offset = offset;
    }

    // @api: ToZeroBased
    public TextFilePosition ToOneBased()
    {
        return new TextFilePosition(1 + Line, 1 + Column, Offset);
    }

    public bool Equals(TextFilePosition other)
    {
        return AreEqual(this, other);
    }

    public static bool AreEqual(TextFilePosition? a, TextFilePosition? b)
    {
        if (a is null) return b is null;
        if (b is null) return a is null;

        if (a.Value.Offset >= 0 && b.Value.Offset >= 0 && a.Value.Offset != b.Value.Offset)
        {
            return false;
        }

        return a.Value.Line == b.Value.Line && a.Value.Column == b.Value.Column;
    }

    public override int GetHashCode()
    {
        return (Line, Column).GetHashCode();
    }

    public override string ToString()
    {
        return $"({Line}, {Column})";
    }

    public int CompareTo(TextFilePosition other)
    {
        return Compare(this, other);
    }

    public static int Compare(TextFilePosition? a, TextFilePosition? b)
    {
        if (a is null && b is not null) return -1;
        if (a is not null && b is null) return 1;
        if (a is null && b is null) return 0;

        Debug.Assert(a is not null);
        Debug.Assert(b is not null);

        var lineComparison = a.Value.Line.CompareTo(b.Value.Line);
        if (lineComparison != 0)
        {
            return lineComparison;
        }

        var columnComparison = a.Value.Column.CompareTo(b.Value.Column);
        if (columnComparison != 0)
        {
            return columnComparison;
        }

        if (a.Value.Offset >= 0 && b.Value.Offset >= 0)
        {
            var offsetComparison = a.Value.Offset.CompareTo(b.Value.Offset);
            if (offsetComparison != 0)
            {
                return offsetComparison;
            }
        }

        return 0;
    }

    public static bool operator <(TextFilePosition a, TextFilePosition b)
    {
        return Compare(a, b) < 0;
    }

    public static bool operator <=(TextFilePosition a, TextFilePosition b)
    {
        return Compare(a, b) <= 0;
    }

    public static bool operator >(TextFilePosition a, TextFilePosition b)
    {
        return Compare(a, b) > 0;
    }

    public static bool operator >=(TextFilePosition a, TextFilePosition b)
    {
        return Compare(a, b) >= 0;
    }
}
