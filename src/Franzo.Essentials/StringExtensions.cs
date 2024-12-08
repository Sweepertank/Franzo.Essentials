using System.Text;

namespace Franzo.Essentials;

public static class StringExtensions
{
    [Obsolete]
    public static bool IsNullOrEqualTo(this string? self, string other)
    {
        return self is null || self == other;
    }

    public static string Capitalized(this string self)
    {
        // https://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-with-maximum-performance
        return self switch
        {
            "" => "",
            _ => string.Concat(self[0].ToString().ToUpper(), self.AsSpan(1))
        };
    }

    public static string Uncapitalized(this string self)
    {
        return self switch
        {
            "" => "",
            _ => string.Concat(self[0].ToString().ToLower(), self.AsSpan(1))
        };
    }

    [Obsolete]
    public static (string, string) SplitAtLast(this string self, char character)
    {
        return (
            self.RemoveStartingAtLastIndexOf(character),
            self.RemoveUpToAndIncludingLastIndexOf(character));
    }

    [Obsolete]
    public static int CountTextElements(this string self)
    {
        return self.AsSpan().CountTextElements();
    }

    public static string RemoveStartingAtFirstIndexOf(this string self, char character)
    {
        var index = self.IndexOf(character);
        if (index < 0)
        {
            return self;
        }

        return self.Remove(index);
    }

    public static string RemoveStartingAtLastIndexOf(this string self, char character)
    {
        var index = self.LastIndexOf(character);
        if (index < 0)
        {
            return self;
        }

        return self.Remove(index);
    }

    public static string RemoveUpToAndIncludingLastIndexOf(this string self, char character)
    {
        var index = self.LastIndexOf(character);
        if (index < 0)
        {
            return self;
        }

        return self.Remove(0, index + 1);
    }

    public static string RemoveUpToAndIncludingFirstIndexOf(this string self, char character)
    {
        var index = self.IndexOf(character);
        if (index < 0)
        {
            return self;
        }

        return self.Remove(0, index + 1);
    }

    public static string Repeat(this string str, int repetitions)
    {
        var sb = new StringBuilder(repetitions * str.Length);
        for (var i = 0; i < repetitions; i++)
        {
            sb.Append(str);
        }

        return sb.ToString();
    }

    [Obsolete]
    public static string GetTypeName(this string fullTypeName)
    {
        return fullTypeName.RemoveUpToAndIncludingLastIndexOf(Type.Delimiter);
    }

    public static string RemoveLastNChars(this string self, int n)
    {
        return self.Remove(self.Length - n, n);
    }

    public static string WithoutSuffix(this string self, string suffix)
    {
        if (!self.EndsWith(suffix))
        {
            throw new ArgumentException(
                $"The given string must end with the given suffix, '{suffix}'. Instead, the given string was '{self}'.",
                nameof(self));
        }

        return self.RemoveLastNChars(suffix.Length);
    }

    // @Robustness: Test this
    [Obsolete]
    public static bool ContainsUnpairedSurrogates(this string self)
    {
        if (self.Length == 1)
        {
            return char.IsSurrogate(self[0]);
        }

        var i = 0;
        while (i < self.Length - 1)
        {
            var c = self[i];
            var d = self[i];

            if (char.IsHighSurrogate(c) && !char.IsLowSurrogate(d))
            {
                return true;
            }
            else if (char.IsLowSurrogate(c))
            {
                return true;
            }

            if (char.IsSurrogatePair(c, d))
            {
                i += 2;
            }
            else
            {
                i += 1;
            }
        }

        return false;
    }
}
