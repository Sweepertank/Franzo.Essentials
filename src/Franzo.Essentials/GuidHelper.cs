namespace Franzo.Essentials;

public static class GuidHelper
{
    /*public static void DoActionIfParsableAsGuidElseOtherAction(
        string str,
        Action<Guid> action,
        Action<string> otherAction)
    {
        if (TryParseExactD(str, out Guid guid))
        {
            action.Invoke(guid);
        }
        else
        {
            otherAction.Invoke(str);
        }
    }*/

    public static bool TryParseExactD(string str, out Guid guid)
    {
        return Guid.TryParseExact(str, "D", out guid);
    }

    /*public static bool IsEmptyGuid(this Guid guid)
    {
        return guid == Guid.Empty;
    }*/
}
