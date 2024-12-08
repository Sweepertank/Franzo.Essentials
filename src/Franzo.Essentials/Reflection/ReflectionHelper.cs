using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class ReflectionHelper
{
    public const BindingFlags PublicInstanceBindingFlags =
        BindingFlags.Public | BindingFlags.Instance;

    public const BindingFlags PublicInstanceIgnoreCaseBindingFlags =
        PublicInstanceBindingFlags | BindingFlags.IgnoreCase;

    public const BindingFlags PublicInstanceStaticDeclaredOnlyBindingFlags =
        PublicInstanceBindingFlags | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static readonly MethodInfo s_createImmutableArrayMethod =
        typeof(ReflectionHelper).GetMethodOrThrow(
            nameof(CreateImmutableArray),
            BindingFlags.Static | BindingFlags.NonPublic);

    private static readonly MethodInfo s_createImmutableListMethod =
        typeof(ReflectionHelper).GetMethodOrThrow(
            nameof(CreateImmutableArray),
            BindingFlags.Static | BindingFlags.NonPublic);

    private static readonly MethodInfo s_createImmutableDictionaryMethod =
        typeof(ReflectionHelper).GetMethodOrThrow(
            nameof(CreateImmutableDictionary),
            BindingFlags.Static | BindingFlags.NonPublic);

    private static readonly MethodInfo s_createImmutableHashSetMethod =
        typeof(ReflectionHelper).GetMethodOrThrow(
            nameof(CreateImmutableHashSet),
            BindingFlags.Static | BindingFlags.NonPublic);

    public static PropertyOrFieldInfo? GetObjectPropertyOrFieldBetter(object obj, string name)
    {
        return obj.GetType().GetPropertyOrFieldBetter(
            name,
            null,
            ReflectionHelper.PublicInstanceBindingFlags);
    }

    [Obsolete]
    public static T? CreateInstance<T>()
    {
        return (T?)CreateInstance(typeof(T));
    }

    [Obsolete]
    public static object? CreateInstance(Type type)
    {
        if (type == typeof(string))
        {
            return "";
        }
        else if (type.IsArray)
        {
            return Array.CreateInstance(type.IReadOnlyCollection_T_ItemType()!, 0);
        }
        else if (type.IsConstructedFromEssentialConcreteImmutableCollectionType())
        {
            return CreateImmutableCollection(type);
        }
        else
        {
            return Activator.CreateInstance(type);
        }
    }

    [Obsolete]
    public static object? CreateInstanceAssignableTo(Type type)
    {
        Type typeToInstantiate = type;
        if (type.ImplementsIEnumerable_T_ExactlyOnce())
        {
            Type itemType = type.IEnumerable_T_ItemType()!;

            Type? genericTypeToInstantiate = null;
            if (typeof(List<>).IsAssignableTo_OpenGenericInclusive(type))
            {
                genericTypeToInstantiate = typeof(List<>);
            }
            if (typeof(Dictionary<,>).IsAssignableTo_OpenGenericInclusive(type))
            {
                genericTypeToInstantiate = typeof(Dictionary<,>);
            }
            else if (typeof(HashSet<>).IsAssignableTo_OpenGenericInclusive(type))
            {
                genericTypeToInstantiate = typeof(HashSet<>);
            }
            else if (typeof(ImmutableArray<>).IsAssignableTo_OpenGenericInclusive(type))
            {
                genericTypeToInstantiate = typeof(ImmutableArray<>);
            }
            else if (typeof(ImmutableList<>).IsAssignableTo_OpenGenericInclusive(type))
            {
                genericTypeToInstantiate = typeof(ImmutableList<>);
            }
            else if (typeof(ImmutableDictionary<,>).IsAssignableTo_OpenGenericInclusive(type))
            {
                genericTypeToInstantiate = typeof(ImmutableDictionary<,>);
            }
            else if (typeof(ImmutableHashSet<>).IsAssignableTo_OpenGenericInclusive(type))
            {
                genericTypeToInstantiate = typeof(ImmutableHashSet<>);
            }

            if (genericTypeToInstantiate is not null)
            {
                Type[] typeArguments;
                if (itemType.IsAssignableTo_OpenGenericInclusive(typeof(KeyValuePair<,>)))
                {
                    typeArguments = new[]
                    {
                        itemType.GenericTypeArguments[0],
                        itemType.GenericTypeArguments[1]
                    };
                }
                else
                {
                    typeArguments = new Type[] { itemType };
                }

                typeToInstantiate = genericTypeToInstantiate.MakeGenericType(typeArguments);
            }
        }

        return CreateInstance(typeToInstantiate);
    }

    [Obsolete]
    public static ICollection CreateImmutableCollection(
        Type immutableCollectionType,
        IEnumerable? items = null)
    {
        if (!immutableCollectionType.IsConstructedFromEssentialConcreteImmutableCollectionType())
        {
            throw new ArgumentException(
                $"The type given for parameter '{nameof(immutableCollectionType)}' must be constructed from one of the essential concrete collection types in the {Full.Nameof(nameof(System.Collections.Immutable))} namespace.",
                nameof(immutableCollectionType));
        }

        var itemType = immutableCollectionType.IReadOnlyCollection_T_ItemType()!;
        if (!itemType.IsAssignableTo(typeof(IEnumerable<>).MakeGenericType(itemType)))
        {
            throw new ArgumentException(
                $"The value given for parameter '{nameof(items)}', if not null, must implement {nameof(IEnumerable<object>)}<T>, where T is equal to the item type of the type given for parameter '{nameof(immutableCollectionType)}'.",
                nameof(items));
        }

        MethodInfo genericCreateMethod;
        if (immutableCollectionType.IsConstructedFromGenericType(typeof(ImmutableArray<>)))
        {
            genericCreateMethod = s_createImmutableArrayMethod;
        }
        else if (immutableCollectionType.IsConstructedFromGenericType(typeof(ImmutableList<>)))
        {
            genericCreateMethod = s_createImmutableListMethod;
        }
        else if (immutableCollectionType.IsConstructedFromGenericType(typeof(ImmutableDictionary<,>)))
        {
            genericCreateMethod = s_createImmutableDictionaryMethod;
        }
        else if (immutableCollectionType.IsConstructedFromGenericType(typeof(ImmutableHashSet<>)))
        {
            genericCreateMethod = s_createImmutableHashSetMethod;
        }
        else
        {
            throw new ShouldNeverBeThrownException();
        }

        var args = items is null ? null : new object?[] { items };
        var createMethod = genericCreateMethod.MakeGenericMethod(itemType);
        return (ICollection)createMethod.Invoke(null, args)!;
    }

    [Obsolete]
    private static ImmutableArray<T> CreateImmutableArray<T>(IEnumerable<T>? items)
    {
        return items?.ToImmutableArray() ?? ImmutableArray<T>.Empty;
    }

    [Obsolete]
    private static ImmutableList<T> CreateImmutableList<T>(IEnumerable<T>? items)
    {
        return items?.ToImmutableList() ?? ImmutableList<T>.Empty;
    }

    [Obsolete]
    private static ImmutableDictionary<TKey, TValue> CreateImmutableDictionary<TKey, TValue>(
        IEnumerable<KeyValuePair<TKey, TValue>>? pairs) where TKey : notnull
    {
        return pairs?.ToImmutableDictionary() ?? ImmutableDictionary<TKey, TValue>.Empty;
    }

    [Obsolete]
    private static ImmutableHashSet<T> CreateImmutableHashSet<T>(IEnumerable<T>? items)
    {
        return items?.ToImmutableHashSet() ?? ImmutableHashSet<T>.Empty;
    }
}
