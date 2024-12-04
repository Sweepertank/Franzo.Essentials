using System.Numerics;

namespace Franzo.Essentials;

public static class NumberExtensions
{
    public static bool IsInRangeInclusive<T>(this T self, T start, T end) where T : INumber<T>
    {
        return self >= start && self <= end;
    }
}
