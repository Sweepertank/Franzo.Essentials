using System.Diagnostics;

namespace Franzo.Essentials;

public readonly struct FilePosition : IComparable<FilePosition>
{
    public static readonly FilePosition Start = new(1, 1, 0);

    public int Line { get; }
    public int Column { get; }
    public int Offset { get; }

    public FilePosition(int line, int column, int offset = -1)
    {
        Line = line;
        Column = column;
        Offset = offset;
    }

    // internal
    public FilePosition ToOneBasedPosition()
    {
        return new FilePosition(1 + Line, 1 + Column, Offset);
    }

    public int CompareTo(FilePosition other)
    {
        return Compare(this, other);
    }

    public override string ToString()
    {
        return $"({Line}, {Column})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not FilePosition other)
        {
            return false;
        }

        return AreEqual(this, other);
    }

    public override int GetHashCode()
    {
        return (Line, Column).GetHashCode();
    }

    public static FilePosition LastPositionIn(string s)
    {
        if (s.EndsWith('\n'))
        {
            s = s[..(s.Length - 1)];
        }

        var offset = System.Math.Max(0, s.Length - 1);
        int zeroBasedLine;
        int zeroBasedColumn;
        if (s.Contains('\n'))
        {
            (var beforeLastLineStr, var lastLine) = s.SliceAfterLast('\n');
            zeroBasedLine = beforeLastLineStr.Count(c => c is '\n');
            zeroBasedColumn = System.Math.Max(0, lastLine.Length - 1);
        }
        else
        {
            zeroBasedLine = 0;
            zeroBasedColumn = offset;
        }

        return new FilePosition(zeroBasedLine, zeroBasedColumn, offset).ToOneBasedPosition();
    }

    public static int Compare(FilePosition? a, FilePosition? b)
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

        return 0;
    }

    public static bool AreEqual(FilePosition? a, FilePosition? b)
    {
        return Compare(a, b) == 0;
    }

    public static bool operator <(FilePosition? a, FilePosition? b)
    {
        return Compare(a, b) < 0;
    }

    public static bool operator <=(FilePosition? a, FilePosition? b)
    {
        return Compare(a, b) <= 0;
    }

    public static bool operator >(FilePosition? a, FilePosition? b)
    {
        return Compare(a, b) > 0;
    }

    public static bool operator >=(FilePosition? a, FilePosition? b)
    {
        return Compare(a, b) >= 0;
    }

    public static bool operator ==(FilePosition? a, FilePosition? b)
    {
        return AreEqual(a, b);
    }

    public static bool operator !=(FilePosition? a, FilePosition? b)
    {
        return !AreEqual(a, b);
    }
}
