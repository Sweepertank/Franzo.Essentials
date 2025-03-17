using System.Globalization;

namespace Franzo.Essentials;

public static class ReadOnlySpanExtensions
{
    /*[Obsolete]
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
    }*/

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

    public static bool HasUnpairedSurrogates(this ReadOnlySpan<char> self)
    {
        // https://stackoverflow.com/questions/50761133/how-to-check-for-invalid-utf-8-characters

        for (var i = 0; i < self.Length; i++)
        {
            var c = self[i];
            var uc = char.GetUnicodeCategory(c);

            if (uc == UnicodeCategory.Surrogate)
            {
                // Unpaired surrogate, like  "😵"[0] + "A" or  "😵"[1] + "A"
                return true;
            }

            // Correct high-low surrogate, we must skip the low surrogate
            // (it is correct because otherwise it would have been a 
            // UnicodeCategory.Surrogate)
            if (char.IsHighSurrogate(c))
            {
                i++;
            }
        }

        return false;
    }
}
