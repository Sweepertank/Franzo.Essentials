using System.Text;

namespace Franzo.Essentials;

public static class NetStandardStringExtensions
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

    public static string Capitalize(this string self)
    {
        return self switch
        {
            "" => "",
            _ => char.ToUpper(self[0]) + self.Substring(1, self.Length - 1)
        };
    }

    public static string Uncapitalize(this string self)
    {
        return self switch
        {
            "" => "",
            _ => char.ToLower(self[0]) + self.Substring(1, self.Length - 1)
        };
    }

    public static (string, string) SliceAfterLast(this string self, char character)
    {
        int index = self.LastIndexOf(character);
        if (index < 0)
        {
            return (self, "");
        }

        return (self.Substring(0, index + 1), self.Substring(index + 1, self.Length - index - 1));
    }
}
