using System.Reflection;

namespace Franzo.Essentials.Reflection;

[Obsolete]
public class NullabilityInfoEqualityComparer : EqualityComparer<NullabilityInfo>
{
    public static readonly NullabilityInfoEqualityComparer Instance = new();

    public override bool Equals(NullabilityInfo? x, NullabilityInfo? y)
    {
        if (x is null) return y is null;
        if (y is null) return x is null;

        return x.Type == y.Type
            && (x.ReadState == y.ReadState
                || x.ReadState is NullabilityState.Unknown
                || y.ReadState is NullabilityState.Unknown)
            && (x.WriteState == y.WriteState
                || x.WriteState is NullabilityState.Unknown
                || y.WriteState is NullabilityState.Unknown)
            && x.ElementType.NullabilityInfoEquals(y.ElementType)
            && x.GenericTypeArguments.SequenceEqual(y.GenericTypeArguments, this);
    }

    public override int GetHashCode(NullabilityInfo obj)
    {
        return obj.GetHashCode();
    }
}
