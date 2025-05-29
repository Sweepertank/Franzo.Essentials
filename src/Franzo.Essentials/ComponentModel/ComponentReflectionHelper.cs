using System.Linq.Expressions;
using System.Reflection;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.ComponentModel;

public static class ComponentReflectionHelper
{
    public static object? CallGetMethod(object? target, MethodInfo getMethod)
    {
        var getter = getMethod.CreateFastGetter<object?>();
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

    private delegate void Setter(object? instance, object? value);
}
