using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class AssemblyExtensions
{
    public static string? GetDirectory(this Assembly self)
    {
        return Path.GetDirectoryName(self.Location);
    }
}
