using System.Linq.Expressions;
using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class MethodInfoExtensions
{
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
                .Concat(new[] { self.ReturnType })
                .ToArray()),
            target);
    }

    public static bool ExistsAndIsPublic(this MethodInfo? self)
    {
        return self is not null && self.IsPublic;
    }
}
