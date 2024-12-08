using System.Linq.Expressions;
using System.Reflection;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.ComponentModel;

public static class ComponentReflectionHelper
{
    private static readonly MethodInfo s_createGetterPrivateMethod =
        typeof(ComponentReflectionHelper).GetMethodOrThrow(
            nameof(CreateGetterGeneric),
            BindingFlags.Static | BindingFlags.NonPublic);

    public static object? CallGetMethod(object? target, MethodInfo getMethod)
    {
        var getter = CreateGetter(getMethod);
        try
        {
            return getter.Invoke(target);
        }
        catch (Exception e)
        {
            throw new TargetInvocationException(e);
        }
    }

    public static void CallSetMethod(object? target, MethodInfo setMethod, object? value)
    {
        var setter = CreateSetter(setMethod);
        try
        {
            setter.Invoke(target, value);
        }
        catch (Exception e)
        {
            throw new TargetInvocationException(e);
        }
    }

    public static void CallInsertMethod(
        object? target,
        MethodInfo insertMethod,
        int index,
        object? value)
    {
        insertMethod.Invoke(target, new object?[]
        {
            index,
            value
        });
    }

    public static void CallRemoveAtMethod(object? target, MethodInfo removeAtMethod, int index)
    {
        removeAtMethod.Invoke(target, new object?[]
        {
            index
        });
    }

    private static Getter CreateGetter(MethodInfo getMethod)
    {
        // https://stackoverflow.com/questions/2490828/createdelegate-with-unknown-types

        if (getMethod.DeclaringType is null)
        {
            throw new ArgumentException();
        }
        else if (getMethod.GetParameters().Length != 0)
        {
            throw new ArgumentException();
        }

        var method = s_createGetterPrivateMethod.MakeGenericMethod(
            getMethod.DeclaringType,
            getMethod.ReturnType);
        var createdDelegate = method.Invoke(
            null,
            new object?[]
            {
                getMethod
            })!;

        return (Getter)createdDelegate;
    }

    private static Getter CreateGetterGeneric<TTarget, TReturn>(MethodInfo getMethod)
    {
        var del = (Func<TTarget, TReturn>)Delegate.CreateDelegate(
            typeof(Func<TTarget, TReturn>),
            null,
            getMethod);

        return target =>
        {
            if (target is not TTarget instance)
            {
                throw new ArgumentException();
            }

            return del.Invoke(instance);
        };
    }

    private static Setter CreateSetter(MethodInfo setMethod)
    {
        // https://stackoverflow.com/questions/10820453/reflection-performance-create-delegate-properties-c

        var parameters = setMethod.GetParameters();
        if (parameters.Length != 1)
        {
            throw new ArgumentException();
        }

        var obj = Expression.Parameter(typeof(object), "o");
        var value = Expression.Parameter(typeof(object));
        var expr = Expression.Lambda<Setter>(
            Expression.Call(
                setMethod.DeclaringType is null
                    ? null
                    : Expression.Convert(obj, setMethod.DeclaringType),
                setMethod,
                Expression.Convert(value, parameters[0].ParameterType)),
            obj,
            value);

        return expr.Compile();
    }

    private delegate object? Getter(object? instance);
    private delegate void Setter(object? instance, object? value);
}
