using System.Runtime.CompilerServices;

namespace Franzo.Essentials;

public static class Full
{
    public static string Nameof(
        string nameofExpression,
        [CallerArgumentExpression(nameof(nameofExpression))] string fullname = "")
    {
        // https://github.com/dotnet/csharplang/discussions/701

        int openParenIndex = fullname.IndexOf('(');
        int closeParenIndex = fullname.IndexOf(')');

        return fullname[(openParenIndex + 1)..closeParenIndex];
    }
}
