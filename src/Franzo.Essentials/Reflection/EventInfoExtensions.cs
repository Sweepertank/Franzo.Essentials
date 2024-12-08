using System.Reflection;

namespace Franzo.Essentials.Reflection;

public static class EventInfoExtensions
{
    public static bool IsStatic(this EventInfo self)
    {
        if (self.AddMethod is null)
        {
            throw new ShouldNeverBeThrownException();
        }

        return self.AddMethod.IsStatic;
    }

    public static NullabilityInfo NullabilityInfo(this EventInfo self)
    {
        var context = new NullabilityInfoContext();
        return context.Create(self);
    }
}
