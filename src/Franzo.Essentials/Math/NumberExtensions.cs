using System.Numerics;

namespace Franzo.Essentials.Math;

public static class NumberExtensions
{
    public static T ClampedBetween<T>(this T self, T min, T max) where T : INumber<T>
    {
        return T.Clamp(self, min, max);
    }

    public static T ClampedToNoLessThan<T>(this T self, T value) where T : INumber<T>
    {
        return T.Max(self, value);
    }

    public static T ClampedToNoGreaterThan<T>(this T self, T value) where T : INumber<T>
    {
        return T.Min(self, value);
    }

    public static T SnappedToNearestMultipleOf<T>(this T self, T value) where T : INumber<T>
    {
        // @Cleanup: is this necessary?
        if (T.IsNaN(self))
        {
            return self;
        }

        var low = (self / value) - (self % value) * value;
        var high = low + T.CopySign(T.One, self) * value;

        if (self < T.Zero)
        {
            (high, low) = (low, high);
        }

        var nearestMultiple = (self - low) < (high - self) ? low : high;
        return nearestMultiple;
    }

    public static bool IsInRangeInclusive<T>(this T self, T min, T max) where T : INumber<T>
    {
        return self >= min && self <= max;
    }

    public static int FirstMultipleGreaterThanOrEqualTo(this int self, int value)
    {
        return (int)System.Math.Ceiling((float)value / self) * self;
    }
}
