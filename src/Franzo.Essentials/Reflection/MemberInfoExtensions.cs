using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Franzo.Essentials;

namespace Franzo.Essentials.Reflection;

public static class MemberInfoExtensions
{
    public static bool IsStatic(this MemberInfo self)
    {
        return self switch
        {
            PropertyInfo property => property.IsStatic(),
            FieldInfo field => field.IsStatic,
            MethodBase method => method.IsStatic,
            EventInfo @event => @event.IsStatic(),
            _ => throw new ArgumentException(
                $"The given {nameof(MemberInfo)} must be either a property, field, method, or event. This is not true of member '{self.Name}'  '{self.DeclaringType}'.",
                nameof(self))
        };
    }

    public static T? GetCustomAttributeBetter<T>(this MemberInfo self) where T : Attribute
    {
        T? attribute;
        try
        {
            attribute = self.GetCustomAttribute<T>();
        }
        catch (AmbiguousMatchException e)
        {
            throw new InvalidAttributeDataException(null, e);
        }

        return attribute;
    }

    public static T? GetCustomAttributeSearchingBaseInterfacesIfInterface<T>(
        this MemberInfo self) where T : Attribute
    {
        if (self is Type { IsInterface: true } interfaceType)
        {
            return interfaceType
                .SelfAndInterfaces()
                .FirstOrDefault(i => i.HasCustomAttribute<T>())
                ?.GetCustomAttributeBetter<T>();
        }
        else
        {
            return self.GetCustomAttributeBetter<T>();
        }
    }

    public static bool HasCustomAttributeSearchingBaseInterfacesIfInterface<T>(
        this MemberInfo self) where T : Attribute
    {
        if (self is Type { IsInterface: true } interfaceType)
        {
            return interfaceType
                .SelfAndInterfaces()
                .SelectMany(i => i.GetCustomAttributes<T>())
                .Any();
        }
        else
        {
            return self.HasCustomAttribute<T>();
        }
    }

    public static bool HasCustomAttribute<T>(this MemberInfo self) where T : Attribute
    {
        return self.GetCustomAttributes<T>().Any();
    }

    public static bool HasDisallowNullAttribute(this MemberInfo self)
    {
        return self.HasCustomAttribute<DisallowNullAttribute>();
    }

    public static bool HasRequiredMemberAttribute(this MemberInfo self)
    {
        return self.HasCustomAttribute<RequiredMemberAttribute>();
    }

    public static bool MemberEquals(this MemberInfo? self, MemberInfo? other)
    {
        return ReflectedTypeInvariantMemberInfoEqualityComparer.Instance.Equals(self, other);
    }

    [Obsolete]
    public static MemberInfo[] GetMembersHiding(this MemberInfo self)
    {
        // https://stackoverflow.com/questions/288357/how-does-reflection-tell-me-when-a-property-is-hiding-an-inherited-member-with-t
        if (self.DeclaringType?.BaseType is null)
        {
            return Array.Empty<MemberInfo>();
        }

        return self.DeclaringType.BaseType.GetMember(self.Name);
    }

    [Obsolete]
    public static IEnumerable<PropertyInfo> GetPropertiesHiding(this MemberInfo self)
    {
        return self.GetMembersHiding().OfType<PropertyInfo>();
    }

    [Obsolete]
    public static bool HidesAnyProperties(this MemberInfo self)
    {
        return self.GetPropertiesHiding().Any();
    }
}
