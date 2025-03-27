using System.Collections.Immutable;
using System.Reflection;
using Franzo.Essentials.Collections;

namespace Franzo.Essentials.Reflection;

public class NullableAwareType
{
    public Type Type { get; }
    public bool IsNullable { get; }
    public ImmutableList<NullableAwareType> GenericTypeArguments { get; }

    private NullableAwareType(
        Type type,
        bool isNullable,
        IEnumerable<NullableAwareType> genericTypeArguments)
    {
        Type = type;
        IsNullable = isNullable;
        GenericTypeArguments = genericTypeArguments.ToImmutableList();
    }

    public NullableAwareType ToNullable()
    {
        var newType = Type;
        IEnumerable<NullableAwareType> newGenericTypeArgs = GenericTypeArguments;
        if (Type.IsValueType && !Type.IsNullableValueType())
        {
            newType = typeof(Nullable<>).MakeGenericType(Type);
            newGenericTypeArgs = new NullableAwareType[] { Type.ToNullableAware() };
        }

        return Create(newType, true, newGenericTypeArgs);
    }

    public NullableAwareType ToNonNullable()
    {
        var newType = Type;
        IEnumerable<NullableAwareType> newGenericTypeArgs = GenericTypeArguments;
        if (Type.IsNullableValueType())
        {
            newType = Type.GenericTypeArguments[0];
            newGenericTypeArgs = [];
        }

        return Create(newType, false, newGenericTypeArgs);
    }

    public bool IsAssignableTo(NullableAwareType other)
    {
        if (!Type.IsAssignableTo(other.Type))
        {
            return false;
        }

        if (IsNullable && !other.IsNullable)
        {
            return false;
        }

        for (var i = 0; i < GenericTypeArguments.Count; i++)
        {
            if (!GenericTypeArguments[i].IsAssignableTo(other.GenericTypeArguments[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not NullableAwareType type)
        {
            return false;
        }

        return Equals(this, type);
    }

    public static bool Equals(NullableAwareType a, NullableAwareType b)
    {
        return a.Type == b.Type
            && a.IsNullable == b.IsNullable
            && a.GenericTypeArguments.SequenceEqual(b.GenericTypeArguments);
    }

    public override int GetHashCode()
    {
        // @todo: make better
        return (Type, IsNullable).GetHashCode();
    }

    public static NullableAwareType Of<T>()
    {
        return NullableAwareTypeGetter<T>.Get();
    }

    internal static NullableAwareType Create(Type type, NullabilityInfo? nullabilityInfo = null)
    {
        if (nullabilityInfo is null)
        {
            return Create(type, "");
        }
        else
        {
            return Create(type, nullabilityInfo, "");
        }
    }

    private static NullableAwareType Create(Type type, string dummy)
    {
        return Create(
            type,
            type.IsNullable(),
            type.GenericTypeArguments.Select(arg => Create(arg)));
    }

    private static NullableAwareType Create(
        Type type,
        NullabilityInfo nullabilityInfo,
        string dummy)
    {
        if (type.IsNullableValueType())
        {
            if (nullabilityInfo.GenericTypeArguments.Length != 0)
            {
                throw new ArgumentException(
                    $"The given {nameof(NullabilityInfo)} must have 0 generic type arguments if the given type is a nullable value type. The given type was '{type}'.",
                    nameof(nullabilityInfo));
            }

            return Create(type);
        }
        else
        {
            if (type.GenericTypeArguments.Length != nullabilityInfo.GenericTypeArguments.Length)
            {
                throw new ArgumentException(
                    $"The given {nameof(NullabilityInfo)} has a different number of generic type arguments ({nullabilityInfo.GenericTypeArguments.Length}) than the given type ('{type}', which has {type.GenericTypeArguments.Length} generic type arguments).",
                    nameof(nullabilityInfo));
            }

            return Create(
                type,
                type.IsNullable(nullabilityInfo),
                type.GenericTypeArguments.IndicesAndItems().Select(
                    pair => Create(pair.Item2, nullabilityInfo.GenericTypeArguments[pair.Item1])));
        }
    }

    private static NullableAwareType Create(
        Type type,
        bool isNullable,
        IEnumerable<NullableAwareType> genericTypeArguments)
    {
        return new NullableAwareType(type, isNullable, genericTypeArguments);
    }

    public static bool operator ==(NullableAwareType a, NullableAwareType b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(NullableAwareType a, NullableAwareType b)
    {
        return !Equals(a, b);
    }

    private static class NullableAwareTypeGetter<T>
    {
        public static readonly PropertyInfo PropertyProperty =
            typeof(NullableAwareTypeGetter<T>).GetPropertyOrThrow(nameof(Property));

        public static T Property
        {
            get => throw new InvalidOperationException();
        }

        public static NullableAwareType Get()
        {
            return PropertyProperty.NullableAwarePropertyType();
        }
    }
}
