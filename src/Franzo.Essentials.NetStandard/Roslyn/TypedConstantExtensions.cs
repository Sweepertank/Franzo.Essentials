using System.Collections;
using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.Roslyn;

public static class TypedConstantExtensions
{
    public static bool TryGetReadableValue<T>(
        this TypedConstant self,
        out T? value)
    {
        if (self.Kind is TypedConstantKind.Error || self.Type is null)
        {
            value = default;
            return false;
        }

        if (self.IsNull)
        {
            value = default;
            return typeof(T).IsClass;
        }
        else if (typeof(T).IsEnum)
        {
            // Should probably also check that the type names are equal between type argument constant type
            // actually no, who cares
            if (Enum.GetUnderlyingType(typeof(T)) == self.Value!.GetType()
                && self.Kind is TypedConstantKind.Enum)
            {
                value = (T)self.Value!;
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }
        else if (typeof(T).IsArray)
        {
            if (self.Kind is TypedConstantKind.Array)
            {
                var recursiveTryGet = typeof(TypedConstantExtensions)
                    .GetMethod(nameof(TryGetReadableValue))!
                    .MakeGenericMethod(typeof(T).GetElementType());

                var array = (IList)Activator.CreateInstance(typeof(T), new object?[] { self.Values.Length });
                for (int i = 0; i < self.Values.Length; i++)
                {
                    var parameters = new object?[] { self.Values[i], null };
                    recursiveTryGet.Invoke(null, parameters);
                    array[i] = parameters[1];
                }

                value = (T)array;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        if (self.Value is not T val)
        {
            value = default;
            return false;
        }

        value = val;
        return true;
    }
}
