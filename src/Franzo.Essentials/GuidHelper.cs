namespace Franzo.Essentials;

public static class GuidHelper
{
    public static bool TryParseExactD(string str, out Guid guid)
    {
        return Guid.TryParseExact(str, "D", out guid);
    }
}
