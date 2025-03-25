using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Franzo.Essentials.Json.Serialization;

public static class Utf8JsonReaderExtensions
{
    public static bool TryGetNonNullString(
        this ref Utf8JsonReader self,
        [NotNullWhen(true)] out string? value)
    {
        if (self.TokenType is not (JsonTokenType.String or JsonTokenType.PropertyName))
        {
            value = null;
            return false;
        }

        value = self.GetString()!;
        return true;
    }
}
