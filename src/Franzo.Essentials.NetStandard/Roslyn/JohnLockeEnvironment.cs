namespace Franzo.Essentials.Roslyn;

public static class JohnLockeEnvironment
{
    public static int ProcessorCount
    {
        get => Environment.ProcessorCount;
    }
}
