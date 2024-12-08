using System.Reflection;

namespace Franzo.Essentials.Reflection;

public class ReflectedTypeInvariantMemberInfoEqualityComparer : IEqualityComparer<MemberInfo>
{
    public static readonly ReflectedTypeInvariantMemberInfoEqualityComparer Instance = new();

    public bool Equals(MemberInfo? x, MemberInfo? y)
    {
        // https://stackoverflow.com/questions/13615927/equality-for-net-propertyinfos

        if (x is null) return y is null;
        if (y is null) return x is null;

        return x.MetadataToken == y.MetadataToken
            && x.Module.Equals(y.Module)
            && x.DeclaringType == y.DeclaringType;
    }

    public int GetHashCode(MemberInfo obj)
    {
        return (obj.MetadataToken, obj.Module, obj.DeclaringType).GetHashCode();
    }
}
