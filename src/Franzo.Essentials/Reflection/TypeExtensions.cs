using System.Collections.Immutable;
using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class TypeExtensions
{
    public static bool IsNullableValueType(this Type self)
    {
        return Nullable.GetUnderlyingType(self) is not null;
    }

    public static bool IsFromAssembly(this Type self, Assembly assembly)
    {
        return self.Assembly == assembly;
    }

    public static bool IsNullable(
        this Type self,
        NullabilityInfo? nullabilityInfo = null)
    {
        if (nullabilityInfo is null or
            {
                ReadState: NullabilityState.Unknown,
                WriteState: NullabilityState.Unknown
            })
        {
            if (self.IsValueType)
            {
                return self.IsNullableValueType();
            }
            else
            {
                return true;
            }
        }

        return nullabilityInfo.ReadState is NullabilityState.Nullable
            || nullabilityInfo.WriteState is NullabilityState.Nullable;
    }

    public static NullableAwareType ToNullableAware(
        this Type self,
        NullabilityInfo? nullabilityInfo = null)
    {
        return NullableAwareType.Create(self, nullabilityInfo);
    }

    public static Type OrObjectType(this Type? self)
    {
        return self ?? typeof(object);
    }

    public static bool IsConstructedFromGenericType(this Type self, Type genericTypeDefinition)
    {
        return self.IsGenericType && self.GetGenericTypeDefinition() == genericTypeDefinition;
    }

    public static bool IsConstructedFromEssentialConcreteImmutableCollectionType(this Type self)
    {
        return self.IsConstructedFromGenericType(typeof(ImmutableArray<>))
            || self.IsConstructedFromGenericType(typeof(ImmutableList<>))
            || self.IsConstructedFromGenericType(typeof(ImmutableDictionary<,>))
            || self.IsConstructedFromGenericType(typeof(ImmutableHashSet<>));
    }

    public static bool IsAbstractOrInterface(this Type self)
    {
        return self.IsAbstract || self.IsInterface;
    }

    public static bool IsConcrete(this Type self)
    {
        return !self.IsAbstractOrInterface() && !self.ContainsGenericParameters;
    }

    public static string FullNameOrName(this Type self)
    {
        return self.FullName ?? self.Name;
    }

    public static bool NameInStyleMatches(this Type self, TypeNameStyle style, string str)
    {
        return (self.Name == str
                && style is TypeNameStyle.Simple or TypeNameStyle.Unspecified)
            || (self.FullNameOrName() == str
                && style is TypeNameStyle.Full or TypeNameStyle.Unspecified)
            || (self.AssemblyQualifiedName == str
                && style is TypeNameStyle.AssemblyQualified or TypeNameStyle.Unspecified);
    }

    public static bool ImplementsInterface(this Type self, Type @interface)
    {
        if (!@interface.IsInterface)
        {
            throw new ArgumentException(
                $"The type given for parameter '{nameof(@interface)}', '{@interface}', is not an interface.",
                nameof(@interface));
        }

        return self.IsAssignableTo_OpenGenericInclusive(@interface);
    }

    public static bool ImplementsInterfaceWithGenericDefinition(
        this Type self,
        Type genericDefinition)
    {
        ThrowIfTypeIsNotGenericDefinition(genericDefinition, nameof(genericDefinition));

        return self.IsAssignableTo_OpenGenericInclusive(genericDefinition);
    }

    public static bool ImplementsIEnumerable_T(this Type self)
    {
        return self.ImplementsInterfaceWithGenericDefinition(typeof(IEnumerable<>));
    }

    public static bool ImplementsIEnumerable_T_MoreThanOnce(this Type self)
    {
        return self.InterfacesWithGenericDefinition(typeof(IEnumerable<>)).Count() > 1;
    }

    public static bool ImplementsIEnumerable_T_ExactlyOnce(this Type self)
    {
        return self.ImplementsIEnumerable_T() && !self.ImplementsIEnumerable_T_MoreThanOnce();
    }

    public static Type? IEnumerable_T_Interface(this Type self)
    {
        var interfaces = self.InterfacesWithGenericDefinition(typeof(IEnumerable<>));
        if (interfaces.Count() > 1)
        {
            throw new AmbiguousMatchException(
                $"The given type '{self}' implements {nameof(IEnumerable<object>)}<T> more than once.");
        }

        return interfaces.FirstOrDefault();
    }

    public static Type? IEnumerable_T_ItemType(this Type self)
    {
        return self.IEnumerable_T_Interface()?.GenericTypeArguments[0];
    }

    public static bool ImplementsIReadOnlyCollection_T(this Type self)
    {
        return self.ImplementsInterfaceWithGenericDefinition(typeof(IReadOnlyCollection<>));
    }

    public static bool ImplementsIReadOnlyCollection_T_MoreThanOnce(this Type self)
    {
        return self.InterfacesWithGenericDefinition(typeof(IReadOnlyCollection<>)).Count() > 1;
    }

    public static bool ImplementsIReadOnlyCollection_T_ExactlyOnce(this Type self)
    {
        return self.ImplementsIReadOnlyCollection_T()
            && !self.ImplementsIReadOnlyCollection_T_MoreThanOnce();
    }

    public static Type? IReadOnlyCollection_T_Interface(this Type self)
    {
        var interfaces = self.InterfacesWithGenericDefinition(typeof(IReadOnlyCollection<>));
        if (interfaces.Count() > 1)
        {
            throw new AmbiguousMatchException(
                $"The given type '{self}' implements {nameof(IReadOnlyCollection<object>)}<T> more than once.");
        }

        return interfaces.FirstOrDefault();
    }

    public static Type? IReadOnlyCollection_T_ItemType(this Type self)
    {
        return self.IReadOnlyCollection_T_Interface()?.GenericTypeArguments[0];
    }

    public static bool ImplementsICollection_T(this Type self)
    {
        return self.ImplementsInterfaceWithGenericDefinition(typeof(ICollection<>));
    }

    public static bool ImplementsIReadOnlyList_T(this Type self)
    {
        return self.ImplementsInterfaceWithGenericDefinition(typeof(IReadOnlyList<>));
    }

    public static Type? ICollection_T_Interface(this Type self)
    {
        var interfaces = self.InterfacesWithGenericDefinition(typeof(ICollection<>));
        if (interfaces.Count() > 1)
        {
            throw new AmbiguousMatchException(
                $"The given type '{self}' implements {nameof(ICollection<object>)}<T> more than once.");
        }

        return interfaces.FirstOrDefault();
    }

    public static Type? IReadOnlyList_T_Interface(this Type self)
    {
        var interfaces = self.InterfacesWithGenericDefinition(typeof(IReadOnlyList<>));
        if (interfaces.Count() > 1)
        {
            throw new AmbiguousMatchException(
                $"The given type '{self}' implements {nameof(IReadOnlyList<object>)}<T> more than once.");
        }

        return interfaces.FirstOrDefault();
    }

    public static Type? IReadOnlyList_T_ItemType(this Type self)
    {
        return self.IReadOnlyList_T_Interface()?.GenericTypeArguments[0];
    }

    public static bool ImplementsIReadOnlyDictionary_TKey_TValue(this Type self)
    {
        return self.ImplementsInterfaceWithGenericDefinition(typeof(IReadOnlyDictionary<,>));
    }

    public static bool ImplementsIReadOnlyDictionary_TKey_TValue_MoreThanOnce(this Type self)
    {
        return self.InterfacesWithGenericDefinition(typeof(IReadOnlyDictionary<,>)).Count() > 1;
    }

    public static bool ImplementsIReadOnlyDictionary_TKey_TValue_ExactlyOnce(this Type self)
    {
        return self.ImplementsIReadOnlyDictionary_TKey_TValue()
            && !self.ImplementsIReadOnlyDictionary_TKey_TValue_MoreThanOnce();
    }

    public static Type? IReadOnlyDictionary_TKey_TValue_Interface(this Type self)
    {
        var interfaces = self.InterfacesWithGenericDefinition(typeof(IReadOnlyDictionary<,>));
        if (interfaces.Count() > 1)
        {
            throw new AmbiguousMatchException(
                $"The given type '{self}' implements {nameof(IReadOnlyDictionary<object, object>)}<T> more than once.");
        }

        return interfaces.FirstOrDefault();
    }

    public static Type? IReadOnlyDictionary_String_TValue_TValueType(this Type self)
    {
        var dictionaryInterface = self.IReadOnlyDictionary_TKey_TValue_Interface();
        if (dictionaryInterface is null || dictionaryInterface.GenericTypeArguments[0] != typeof(string))
        {
            return null;
        }

        return dictionaryInterface = dictionaryInterface.GenericTypeArguments[1];
    }

    public static Type? IDictionary_TKey_TValue_Interface(this Type self)
    {
        var interfaces = self.InterfacesWithGenericDefinition(typeof(IDictionary<,>));
        if (interfaces.Count() > 1)
        {
            throw new AmbiguousMatchException(
                $"The given type '{self}' implements {nameof(IDictionary<object, object>)}<T> more than once.");
        }

        return interfaces.FirstOrDefault();
    }

    public static bool ImplementsIDictionary_TKey_TValue(this Type self)
    {
        return self.ImplementsInterfaceWithGenericDefinition(typeof(IDictionary<,>));
    }

    public static bool ImplementsIDictionary_TKey_TValue_MoreThanOnce(this Type self)
    {
        return self.InterfacesWithGenericDefinition(typeof(IDictionary<,>)).Count() > 1;
    }

    public static bool ImplementsIDictionary_TKey_TValue_ExactlyOnce(this Type self)
    {
        return self.ImplementsIDictionary_TKey_TValue()
            && !self.ImplementsIDictionary_TKey_TValue_MoreThanOnce();
    }

    [Obsolete]
    public static bool ImplementsIReadOnlySet_T(this Type self)
    {
        return self.ImplementsInterfaceWithGenericDefinition(typeof(IReadOnlySet<>));
    }

    public static bool IsAncestorOf(this Type self, Type other)
    {
        return other.AncestorTypes().Any(t => t == self);
    }

    public static IEnumerable<Type> AncestorTypes(this Type self)
    {
        var currentType = self;
        while (currentType is not null)
        {
            yield return currentType;
            currentType = currentType.BaseType;
        }
    }

    public static bool IsProperAncestorOf(this Type self, Type other)
    {
        return self.IsAncestorOf(other) && self != other;
    }

    public static IEnumerable<Type> ProperAncestorTypes(this Type self)
    {
        var enumerator = self.AncestorTypes().GetEnumerator();
        enumerator.MoveNext();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    public static IEnumerable<Type> SelfAndInterfaces(this Type self)
    {
        return new List<Type>() { self }.Union(self.GetInterfaces());
    }

    public static IEnumerable<Type> AncestorTypesAndInterfaces(this Type self)
    {
        return self.AncestorTypes().Union(self.GetInterfaces());
    }

    public static Type? FirstAncestorType(this Type self, Predicate<Type> predicate)
    {
        return self.AncestorTypes().FirstOrDefault(predicate.Invoke);
    }

    public static Type? FirstAncestorTypeWithGenericDefinition(this Type self, Type genericDefinition)
    {
        return self.FirstAncestorType(
            t => t.IsGenericType && t.GetGenericTypeDefinition() == genericDefinition);
    }

    public static bool IsAssignableTo_OpenGenericInclusive(this Type self, Type type)
    {
        return self.FirstAncestorOrInterfaceTypeEqualToOrWithGenericDefinitionEqualTo(type) is not null;
    }

    public static Type? FirstInterfaceWithGenericDefinition(this Type self, Type genericDefinition)
    {
        ThrowIfTypeIsNotGenericDefinition(genericDefinition, nameof(genericDefinition));

        if (!genericDefinition.IsInterface)
        {
            return null;
        }

        return self.FirstAncestorOrInterfaceTypeEqualToOrWithGenericDefinitionEqualTo(genericDefinition);
    }

    public static Type? FirstAncestorOrInterfaceTypeEqualToOrWithGenericDefinitionEqualTo(
        this Type self,
        Type type)
    {
        return self
            .AncestorOrInterfaceTypesEqualToOrWithGenericDefinitionEqualTo(type)
            .FirstOrDefault();
    }

    public static Type? AncestorTypeWithGenericDefinition(this Type self, Type genericDefinition)
    {
        ThrowIfTypeIsNotGenericDefinition(genericDefinition, nameof(genericDefinition));

        if (genericDefinition.IsInterface)
        {
            return null;
        }

        return self.FirstAncestorOrInterfaceTypeEqualToOrWithGenericDefinitionEqualTo(genericDefinition);
    }

    public static IEnumerable<Type> InterfacesWithGenericDefinition(this Type self, Type genericDefinition)
    {
        ThrowIfTypeIsNotGenericDefinition(genericDefinition, nameof(genericDefinition));

        return self.AncestorOrInterfaceTypesEqualToOrWithGenericDefinitionEqualTo(genericDefinition);
    }

    public static IEnumerable<Type> AncestorOrInterfaceTypesEqualToOrWithGenericDefinitionEqualTo(
        this Type self,
        Type type)
    {
        return self.AncestorOrInterfaceTypesEqualToOrWithGenericDefinitionEqualToCore(type).Distinct();
    }

    public static MemberInfo? GetMemberBetter(this Type self, string name, MemberTypes type, BindingFlags bindingFlags)
    {
        var members = self.GetMember(name, type, bindingFlags);
        if (members.Length > 1)
        {
            throw new AmbiguousMatchException(
                $"More than one member on the given type '{self}' was found matching the given name '{name}'.");
        }
        else if (members.Length < 1)
        {
            return null;
        }

        return members[0];
    }

    public static MemberInfo? GetMemberBetter(
        this Type self,
        string name,
        Predicate<MemberInfo>? predicate,
        MemberTypes type,
        BindingFlags bindingFlags)
    {
        predicate ??= PredicateHelper.Tautology<MemberInfo>();

        IEnumerable<Type> typesToEnumerate;
        if (self.IsInterface)
        {
            typesToEnumerate = self.SelfAndInterfaces();
        }
        else
        {
            typesToEnumerate = self.AncestorTypes();
        }

        return typesToEnumerate
            .Select(t => t.GetMemberBetter(name, type, bindingFlags | BindingFlags.DeclaredOnly))
            .FirstOrDefault(member => member is not null && predicate.Invoke(member));
    }

    public static PropertyInfo? GetInstancePropertyBetter(
        this Type self,
        string name,
        Predicate<PropertyInfo>? predicate = null)
    {
        return self.GetPropertyBetter(name, predicate, ReflectionHelper.PublicInstanceBindingFlags);
    }

    public static PropertyInfo? GetPropertyBetter(
        this Type self,
        string name,
        Predicate<PropertyInfo>? predicate = null,
        BindingFlags bindingFlags = NetStandardReflectionHelper.DefaultBindingFlags)
    {
        predicate ??= PredicateHelper.Tautology<PropertyInfo>();

        return (PropertyInfo?)self.GetMemberBetter(
            name,
            m => predicate.Invoke((PropertyInfo)m),
            MemberTypes.Property,
            bindingFlags);
    }

    public static IEnumerable<PropertyInfo> GetInstanceProperties(this Type self)
    {
        return self.GetProperties().Where(p => !p.IsStatic());
    }

    public static FieldInfo? GetFieldBetter(
        this Type self,
        string name,
        Predicate<FieldInfo>? predicate = null,
        BindingFlags bindingFlags = NetStandardReflectionHelper.DefaultBindingFlags)
    {
        predicate ??= PredicateHelper.Tautology<FieldInfo>();

        return (FieldInfo?)self.GetMemberBetter(
            name,
            m => predicate.Invoke((FieldInfo)m),
            MemberTypes.Field,
            bindingFlags);
    }

    public static PropertyOrFieldInfo? GetInstancePropertyOrFieldBetter(
        this Type self,
        string name,
        Predicate<PropertyOrFieldInfo>? typePredicate = null)
    {
        return self.GetPropertyOrFieldBetter(
            name,
            typePredicate,
            ReflectionHelper.PublicInstanceBindingFlags);
    }

    public static PropertyOrFieldInfo? GetPropertyOrFieldBetter(
        this Type self,
        string name,
        Predicate<PropertyOrFieldInfo>? predicate = null,
        BindingFlags bindingFlags = NetStandardReflectionHelper.DefaultBindingFlags)
    {
        predicate ??= PredicateHelper.Tautology<PropertyOrFieldInfo>();

        var property = self.GetPropertyBetter(name, p => predicate.Invoke(p), bindingFlags);
        if (property is not null)
        {
            return property;
        }

        var field = self.GetFieldBetter(name, f => predicate.Invoke(f), bindingFlags);
        if (field is not null)
        {
            return field;
        }

        return null;
    }

    public static IEnumerable<PropertyOrFieldInfo> GetPropertiesAndFields(
        this Type self,
        BindingFlags bindingFlags = NetStandardReflectionHelper.DefaultBindingFlags)
    {
        return self.GetProperties(bindingFlags).Select(p => (PropertyOrFieldInfo)p)
            .Union(self.GetFields(bindingFlags).Select(f => (PropertyOrFieldInfo)f));
    }

    public static IEnumerable<PropertyOrFieldInfo> GetInstancePropertiesAndFields(this Type self)
    {
        return self.GetPropertiesAndFields().Where(p => !p.IsStatic);
    }

    /*public static MethodInfo GetMethodOrThrow(
        this Type self,
        string name,
        BindingFlags bindingFlags = NetStandardReflectionHelper.DefaultBindingFlags)
    {
        return self.GetMethod(name, bindingFlags)
            ?? throw new InvalidOperationException($"The specified method '{name}' could not be found on type '{self}'.");
    }*/

    public static ConstructorInfo? GetDefaultConstructor(this Type self)
    {
        return self.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0);
    }

    public static bool HasDefaultConstructor(this Type self)
    {
        return self.GetDefaultConstructor() is not null;
    }

    // @Cleanup: should this be here or somewhere in AutoInterfaceInheritance instead? and come up with a better name?
    // and consider edge cases of using this in IFamlObject
    /*[Obsolete]
    public static MemberInfo? MostDerivedInheritanceOfMember(this Type self, MemberInfo member)
    {
        if (!self.IsAssignableTo(member.DeclaringType))
        {
            return null;
        }

        foreach (var type in self.AncestorTypes())
        {
            const BindingFlags bindingFlags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.Static | BindingFlags.DeclaredOnly;

            foreach (var typeMember in type.GetMembers(bindingFlags))
            {
                var attribute = typeMember.GetCustomAttributeBetter<InheritedFromAttribute>();
                if (typeMember.Name == member.Name
                    && typeMember.MemberType == member.MemberType
                    && attribute is not null
                    && attribute.Type.IsAssignableTo(member.DeclaringType))
                {
                    return typeMember;
                }
            }
        }

        return member;
    }*/

    public static string NameWithoutAttributeSuffix(this Type self)
    {
        return self.NameWithoutSuffix(nameof(Attribute));
    }

    public static string NameWithoutSuffix(this Type self, string suffix)
    {
        if (!self.Name.EndsWith(suffix))
        {
            throw new ArgumentException(
                $"The given type's name must end with the given suffix, '{suffix}'. The given type was '{self}'.",
                nameof(self));
        }

        return self.Name.WithoutSuffix(suffix);
    }

    /*[Obsolete]
    public static object? CreateDefaultValue(this Type self, NullabilityInfo? nullabilityInfo = null)
    {
        return self.IsNullable(nullabilityInfo)
            ? null
            : ReflectionHelper.CreateInstanceAssignableTo(self);
    }*/

    private static void ThrowIfTypeIsNotGenericDefinition(Type type, string paramName)
    {
        if (!type.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                $"The type given for parameter '{paramName}', '{type}', is not a generic type definition.",
                paramName);
        }
    }

    private static IEnumerable<Type> AncestorOrInterfaceTypesEqualToOrWithGenericDefinitionEqualToCore(
        this Type self,
        Type type)
    {
        // Alternate (uglier but more descriptive) function name:
        // GetFirstAncestorOrInterfaceTypeEqualToOrWhoseGenericDefinitionEqualToTypeOrTypeGenericDefinition

        // https://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class

        var currentType = self;
        while (currentType != null && currentType != typeof(object))
        {
            if (currentType == type
                || (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == type)
                || (type.IsGenericType && currentType == type.GetGenericTypeDefinition())
                || (currentType.IsGenericType
                    && type.IsGenericType
                    && currentType.GetGenericTypeDefinition() == type.GetGenericTypeDefinition()))
            {
                yield return currentType;
            }

            // @Performance: This can probably just happen before the while loop, called just on 'self'
            foreach (var interfaceType in currentType.GetInterfaces())
            {
                var interfaceResult = interfaceType.FirstAncestorOrInterfaceTypeEqualToOrWithGenericDefinitionEqualTo(type);
                if (interfaceResult is not null)
                {
                    yield return interfaceResult;
                }
            }

            currentType = currentType.BaseType;
        }
    }
}
