using System.Reflection;
using OneOf;

namespace Franzo.Essentials.Reflection;

public class PropertyOrFieldInfo :
    OneOfBase<PropertyInfo, FieldInfo>,
    IEquatable<PropertyOrFieldInfo>
{
    public string Name
    {
        get => Member.Name;
    }

    public Type? DeclaringType
    {
        get => Member.DeclaringType;
    }

    public bool IsStatic
    {
        get => Member.IsStatic();
    }

    public MemberInfo Member
    {
        get => (MemberInfo)Value;
    }

    public Type PropertyOrFieldType
    {
        get => Member.ReturnType();
    }

    public NullableAwareType NullableAwarePropertyOrFieldType
    {
        get => Member.NullableAwareReturnType();
    }

    public PropertyOrFieldInfo(OneOf<PropertyInfo, FieldInfo> _) : base(_)
    {
    }

    public bool HasCustomAttribute<T>() where T : Attribute
    {
        return Member.HasCustomAttribute<T>();
    }

    public object? GetValueBetter(object? target, bool failIfNoPublicGetter = false)
    {
        if (Member is PropertyInfo property)
        {
            return property.GetValueBetter(target, failIfNoPublicGetter);
        }
        else if (Member is FieldInfo field)
        {
            return field.GetValueBetter(target);
        }
        else
        {
            throw new ShouldNeverBeThrownException();
        }
    }

    public void SetValueBetter(object? target, object? value, bool failIfSetterNotPublic = false)
    {
        if (Member is PropertyInfo property)
        {
            property.SetValueBetter(target, value, failIfSetterNotPublic);
        }
        else if (Member is FieldInfo field)
        {
            field.SetValueBetter(target, value);
        }
        else
        {
            throw new ShouldNeverBeThrownException();
        }
    }

    public bool Equals(PropertyOrFieldInfo? other)
    {
        return Equals(this, other);
    }

    public override bool Equals(object obj)
    {
        if (obj is not PropertyOrFieldInfo other)
        {
            return false;
        }

        return Equals(other);
    }

    public override int GetHashCode()
    {
        return ReflectedTypeInvariantMemberInfoEqualityComparer.Instance.GetHashCode(Member);
    }

    public static PropertyOrFieldInfo FromMember(MemberInfo member)
    {
        if (member is PropertyInfo property)
        {
            return property;
        }
        else if (member is FieldInfo field)
        {
            return field;
        }
        else
        {
            throw new ArgumentException(
                $"The given member must be a property or field, which is not true of member '{member.Name}' on type '{member.DeclaringType}'.",
                nameof(member));
        }
    }

    public static bool Equals(PropertyOrFieldInfo? a, PropertyOrFieldInfo? b)
    {
        return ReflectedTypeInvariantMemberInfoEqualityComparer.Instance.Equals(
            a?.Member,
            b?.Member);
    }

    public static implicit operator PropertyOrFieldInfo(PropertyInfo _)
        => new PropertyOrFieldInfo(_);

    public static explicit operator PropertyInfo(PropertyOrFieldInfo _)
        => _.AsT0;

    public static implicit operator PropertyOrFieldInfo(FieldInfo _)
        => new PropertyOrFieldInfo(_);

    public static explicit operator FieldInfo(PropertyOrFieldInfo _)
        => _.AsT1;
}
