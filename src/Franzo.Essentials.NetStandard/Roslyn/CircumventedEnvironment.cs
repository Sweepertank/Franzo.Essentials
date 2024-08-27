namespace Franzo.Essentials.Roslyn;

public static class CircumventedEnvironment
{
    public static int ProcessorCount
    {
        get => Environment.ProcessorCount;
    }
}
