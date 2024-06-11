using System.Text;

namespace Franzo.Essentials;

public static class StringExtensions
{
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
        for (int i = 0; i < repetitions; i++)
        {
            sb.Append(str);
        }

        return sb.ToString();
    }
}
