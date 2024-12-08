namespace Franzo.Essentials.Serialization;

// don't use until we make autointerfaceinheritance use franzo.reflection bettergetcustomattribute 
/*[Obsolete]
public class SerializationPropertyOrFieldInfoEqualityComparer : IEqualityComparer<PropertyOrFieldInfo>
{
    public static readonly ReflectedTypeInvariantMemberInfoEqualityComparer Instance = new();

    public bool Equals(PropertyOrFieldInfo? x, PropertyOrFieldInfo? y)
    {
        if (x is null) return y is null;
        if (y is null) return x is null;

        if (x.Member.MemberType != y.Member.MemberType)
        {
            return false;
        }

        if (x.Member.DeclaringType is not null
            && y.Member.DeclaringType is not null
            && ((x.Member.DeclaringType.IsInterface
                 && !y.Member.DeclaringType.IsInterface
                 && y.Member.HasInheritedFromAttribute())
                || (!x.Member.DeclaringType.IsInterface
                    && x.Member.DeclaringType.HasInheritedFromAttribute()
                    && y.Member.DeclaringType.IsInterface)))
        {
            var interfaceMember = x;
            var nonInterfaceMember = y;

            if (!interfaceMember.Member.DeclaringType.IsInterface)
            {
                interfaceMember = y;
                nonInterfaceMember = x;
            }

            var typeNonInterfaceMemberWasInheritedFrom =
                nonInterfaceMember.Member.TypeInheritedFrom_BasedOnAttribute()!;
            MemberInfo? matchingMember;
            var bindingFlags = BindingFlags.Instance | BindingFlags.Static
                | BindingFlags.Public | BindingFlags.NonPublic;

            if (nonInterfaceMember.Member is PropertyInfo)
            {
                matchingMember = typeNonInterfaceMemberWasInheritedFrom.GetProperty(
                    nonInterfaceMember.Member.Name,
                    bindingFlags);
            }
            else
            {
                matchingMember = typeNonInterfaceMemberWasInheritedFrom.GetField(
                    nonInterfaceMember.Member.Name,
                    bindingFlags);
            }

            return matchingMember is not null
                && ReflectedTypeInvariantMemberInfoEqualityComparer.Instance.Equals(
                    matchingMember,
                    interfaceMember.Member);
        }
        else
        {
            return ReflectedTypeInvariantMemberInfoEqualityComparer.Instance.Equals(
                x.Member,
                y.Member);
        }
    }

    public int GetHashCode(PropertyOrFieldInfo obj)
    {
        return 0;
    }
}*/
