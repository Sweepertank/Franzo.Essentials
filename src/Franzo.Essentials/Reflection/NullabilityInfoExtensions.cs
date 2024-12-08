using System.Reflection;

namespace Franzo.Essentials.Reflection;

[Obsolete]
public static class NullabilityInfoExtensions
{
    public static bool NullabilityInfoEquals(this NullabilityInfo? self, NullabilityInfo? other)
    {
        return NullabilityInfoEqualityComparer.Instance.Equals(self, other);
    }

    public static int NullabilityInfoGetHashCode(this NullabilityInfo self)
    {
        return NullabilityInfoEqualityComparer.Instance.GetHashCode(self);
    }
}
