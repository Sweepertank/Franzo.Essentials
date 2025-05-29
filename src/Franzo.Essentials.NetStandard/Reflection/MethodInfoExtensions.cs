using System.Linq.Expressions;
using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class MethodInfoExtensions
{
    private static readonly MethodInfo s_createFastGetterGenericMethod =
        typeof(MethodInfoExtensions).GetMethod(
            nameof(CreateFastGetterGeneric),
            BindingFlags.Static | BindingFlags.NonPublic);

    public static Delegate CreateDelegate(this MethodInfo self, object? target)
    {
        if (self.IsGenericMethod)
        {
            throw new ArgumentException(
                $"The given method cannot be generic. The given method was '{self.Name}' on type '{self.DeclaringType}'.",
                nameof(self));
        }

        return self.CreateDelegate(
            Expression.GetDelegateType(
                self.GetParameters()
                .Select(p => p.ParameterType)
                .Concat([self.ReturnType])
                .ToArray()),
            target);
    }

    public static FastGetter<T> CreateFastGetter<T>(this MethodInfo self)
    {
        // https://stackoverflow.com/questions/2490828/createdelegate-with-unknown-types

        if (self.DeclaringType is null)
        {
            throw new ArgumentException();
        }
        else if (self.GetParameters().Length != 0)
        {
            throw new ArgumentException();
        }

        var method = s_createFastGetterGenericMethod.MakeGenericMethod(
            self.DeclaringType,
            self.ReturnType);
        var createdDelegate = method.Invoke(
            null,
            [self])!;

        return (FastGetter<T>)createdDelegate;
    }

    private static FastGetter<TReturn> CreateFastGetterGeneric<TTarget, TReturn>(
        this MethodInfo self)
    {
        var del = (Func<TTarget, TReturn>)Delegate.CreateDelegate(
            typeof(Func<TTarget, TReturn>),
            null,
            self);

        return target =>
        {
            if (target is not TTarget instance)
            {
                throw new ArgumentException();
            }

            return del.Invoke(instance);
        };
    }

    public static bool ExistsAndIsPublic(this MethodInfo? self)
    {
        return self is not null && self.IsPublic;
    }
}
