using System.Reflection;

namespace Franzo.Essentials;

public static class Helpers
{
    public const int MillisecondsPerSecond = 1000;

    public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    public static float MillisecondsToSeconds(float milliseconds)
    {
        return milliseconds / MillisecondsPerSecond;
    }
}
