using System.Reflection;
using Franzo.Essentials.Reflection;

namespace Franzo.Essentials.ComponentModel;

public static class ParameterInfoExtensions
{
    public static PropertyOrFieldInfo? CorrespondingPropertyOrField(this ParameterInfo self)
    {
        // @todo: maybe a CorrespondingProperty attribute that this method could also look at,
        // in cases where the parameter name doesn't match the property

        if (self.Name is null) return null;
        else if (self.Member.DeclaringType is null) return null;
        else if (self.Name.Length == 0 || !char.IsLower(self.Name[0]))
        {
            return null;
        }

        // @todo: type predicate should account for the possibility that the property type is more general than the parameter,
        // e.g. parameter is type string and property is type object, parameter is type IList<> and property is type IReadOnlyList<> (ones like this are a pain because IList doesn't actually implement IReadOnlyList)
        // parameter is type IEnumerable<> and property is type ImmutableList<>/IImmutableList<> (also a pain)

        var capitalizedName = self.Name.Capitalize();
        return self.Member.DeclaringType.GetInstancePropertyOrFieldBetter(
            capitalizedName,
            p =>
            {
                if (p.NullableAwarePropertyOrFieldType == self.NullableAwareParameterType())
                {
                    return true;
                }

                return self.TypeIsEnumerableInterfaceOfHopefullyReadOnlyCollectionType(
                    p.PropertyOrFieldType);
            });
    }
}
