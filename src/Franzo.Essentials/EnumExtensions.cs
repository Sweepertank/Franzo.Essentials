namespace Franzo.Essentials;

public static class EnumExtensions
{
    /*public static Enum UnsetFlag(this Enum self, Enum flag)
    {
    }*/
    /*public static T WithFlags<T>(this T self, T flags) where T : struct, Enum
    {
        var intSelf = (int)(object)self;
        var intFlags = (int)(object)flags;
        return (T)(object)(intSelf | intFlags);
    }

    public static T WithoutFlags<T>(this T self, T flag) where T : struct, Enum
    {
        var intSelf = (int)(object)self;
        var intFlags = (int)(object)flag;
        return (T)(object)(intSelf & ~intFlags);
    }*/

    public static IEnumerable<T> Flags<T>(this T self) where T : struct, Enum
    {
        return Enum.GetValues<T>().Where(v => self.HasFlag(v));
    }

    public static bool HasFlagsOtherThan<T>(this T self, T flags) where T : struct, Enum
    {
        return self.Flags().Where(f => !flags.HasFlag(f)).Any();
    }
}
