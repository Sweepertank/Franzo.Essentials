namespace Franzo.Essentials;

public static class StringExtensionsASF
{
    [Obsolete]
    public static bool IsNullOrEqualTo(this string? self, string other)
    {
        return self is null || self == other;
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

    public static string[] ParseKebabCase(this string self)
    {
        return self.Split('-');
    }

    public static string KebabToPascalCase(this string self)
    {
        return string.Join(null, self.ParseKebabCase().Select(s => s.Capitalize()));
    }
}
