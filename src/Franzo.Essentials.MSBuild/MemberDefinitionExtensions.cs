using Mono.Cecil;

namespace Franzo.Essentials.MSBuild;

internal static class MemberDefinitionExtensions
{
    public static bool IsPossiblyJrifConstructTyped(this IMemberDefinition self)
    {
        var type = self switch
        {
            FieldDefinition field => field.FieldType,
            PropertyDefinition property => property.PropertyType,
            _ => throw new ShouldNeverBeThrownException()
        };

        return type.Name.EndsWith("Symbol");
    }
}
