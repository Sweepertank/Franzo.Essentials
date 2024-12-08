using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class FieldInfoExtensions
{
    public static NullabilityInfo NullabilityInfo(this FieldInfo self)
    {
        var context = new NullabilityInfoContext();
        return context.Create(self);
    }

    public static NullableAwareType NullableAwareFieldType(this FieldInfo self)
    {
        return self.FieldType.ToNullableAware(self.NullabilityInfo());
    }

    public static object? GetValueBetter(this FieldInfo self, object? target)
    {
        return self.GetValue(target);
    }

    public static void SetValueBetter(this FieldInfo self, object? target, object? value)
    {
        if (self.IsInitOnly)
        {
            throw new InvalidOperationException(
                $"Cannot set value of the given field '{self.Name}' on type '{self.DeclaringType}' because it is init-only.");
        }

        self.SetValue(target, value);
    }
}
