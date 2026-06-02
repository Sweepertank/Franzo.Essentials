namespace Franzo.Essentials;

public static class StringExtensions
{
    public static StringSpan ToStringSpan(this string self)
    {
        return new StringSpan(self, 0, self.Length);
    }

    public static StringSpan ToStringSpan(this string self, int start, int length)
    {
        return new StringSpan(self, start, length);
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

    public static string[] ParseKebabCase(this string self)
    {
        return self.Split('-');
    }

    public static string KebabToPascalCase(this string self)
    {
        return string.Join(null, self.ParseKebabCase().Select(s => s.Capitalize()));
    }

    public static bool HasUnpairedSurrogates(this string self)
    {
        return self.AsSpan().HasUnpairedSurrogates();
    }

    public static TextFilePosition LastFilePosition(this string self)
    {
        if (self.EndsWith('\n'))
        {
            self = self[..(self.Length - 1)];
        }

        var offset = System.Math.Max(0, self.Length - 1);
        int zeroBasedLine;
        int zeroBasedColumn;
        if (self.Contains('\n'))
        {
            (var beforeLastLineStr, var lastLine) = self.SliceAfterLast("\n");
            zeroBasedLine = beforeLastLineStr.Count(c => c is '\n');
            zeroBasedColumn = System.Math.Max(0, lastLine.Length - 1);
        }
        else
        {
            zeroBasedLine = 0;
            zeroBasedColumn = offset;
        }

        return new TextFilePosition(zeroBasedLine, zeroBasedColumn, offset).ToOneBased();
    }
}
