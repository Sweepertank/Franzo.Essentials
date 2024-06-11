using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.Roslyn;

public static class AttributeDataExtensions
{
    public static Location? Location(this AttributeData self)
    {
        return self.ApplicationSyntaxReference?.GetSyntax().GetLocation();
    }

    public static bool TryGetReadableValueFromConstructorArgument<T>(
        this AttributeData self,
        int argumentIndex,
        out T? value)
    {
        if (argumentIndex >= self.ConstructorArguments.Length)
        {
            value = default;
            return false;
        }

        return self.ConstructorArguments[argumentIndex].TryGetReadableValue(
            out value);
    }
}
