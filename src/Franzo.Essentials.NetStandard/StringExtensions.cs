using System.Text;

namespace Franzo.Essentials;

public static class StringExtensions
{
    public static string RemoveStartingAtFirst(this string self, char c)
    {
        var index = self.IndexOf(c);
        if (index < 0)
        {
            return self;
        }

        return self.Remove(index);
    }

    public static string RemoveStartingAtLast(this string self, char c)
    {
        var index = self.LastIndexOf(c);
        if (index < 0)
        {
            return self;
        }

        return self.Remove(index);
    }

    public static string RemoveUpToAndIncludingLast(this string self, char c)
    {
        var index = self.LastIndexOf(c);
        if (index < 0)
        {
            return self;
        }

        return self.Remove(0, index + 1);
    }

    public static string RemoveUpToAndIncludingFirst(this string self, char c)
    {
        var index = self.IndexOf(c);
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
