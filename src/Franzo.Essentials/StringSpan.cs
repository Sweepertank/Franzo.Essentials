using System.Collections;

namespace Franzo.Essentials;

public readonly struct StringSpan : IReadOnlyList<char>
{
    public static readonly StringSpan Empty = new("", 0, 0);

    public string String { get; }
    public int Start { get; }
    public int Length { get; }

    public int End
    {
        get => Start + Length;
    }

    public char this[int index]
    {
        get => String[Start + index];
    }

    int IReadOnlyCollection<char>.Count
    {
        get => Length;
    }

    public StringSpan(string @string, int start, int length)
    {
        String = @string;
        Start = start;
        Length = length;

        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, null);
        }
        else if (start < 0 || start > @string.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(start), start, null);
        }
        else if (End < 0 || End > @string.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, null);
        }
    }

    public StringSpan Slice(int start, int length)
    {
        return new StringSpan(String, Start + start, length);
    }

    public ReadOnlySpan<char> ToSpan()
    {
        return String.AsSpan(Start, Length);
    }

    public IEnumerator<char> GetEnumerator()
    {
        return new Enumerator(this);
    }

    public override string ToString()
    {
        return String.Substring(Start, Length);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private class Enumerator : IEnumerator<char>
    {
        public StringSpan Span { get; }
        public int CurrentIndex { get; private set; }

        public char Current
        {
            get => Span[CurrentIndex];
        }

        object IEnumerator.Current
        {
            get => Current;
        }

        public Enumerator(StringSpan span)
        {
            Span = span;
            CurrentIndex = 0;
        }

        public bool MoveNext()
        {
            if (CurrentIndex == Span.Length)
            {
                return false;
            }

            CurrentIndex++;
            return true;
        }

        public void Reset()
        {
            CurrentIndex = 0;
        }

        public void Dispose()
        {
        }
    }
}
