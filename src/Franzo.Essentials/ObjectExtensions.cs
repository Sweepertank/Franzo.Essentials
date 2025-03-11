using System.Text;

namespace Franzo.Essentials;

public static class ObjectExtensions
{
    public static string Repeat(this object self, int count)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < count; i++)
        {
            sb.Append(self);
        }

        return sb.ToString();
    }
}
