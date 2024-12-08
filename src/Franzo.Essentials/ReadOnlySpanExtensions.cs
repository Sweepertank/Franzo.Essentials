using System.Globalization;

namespace Franzo.Essentials;

public static class ReadOnlySpanExtensions
{
    [Obsolete]
    public static int CountTextElements(this ReadOnlySpan<char> self)
    {
        var count = 0;
        var i = 0;
        while (i < self.Length)
        {
            var remaining = self[i..];
            i += StringInfo.GetNextTextElementLength(remaining);
            count++;
        }

        return count;
    }

    public static int IndexOf<T>(this ReadOnlySpan<T> self, Predicate<T> predicate)
    {
        for (var i = 0; i < self.Length; i++)
        {
            var el = self[i];
            if (predicate.Invoke(el))
            {
                return i;
            }
        }

        return -1;
    }
}
