namespace Franzo.Essentials;

public static class ConsoleHelper
{
    public static void WithForegroundColor(ConsoleColor color, Action action)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        action.Invoke();
        Console.ForegroundColor = oldColor;
    }

    public static void WithBackgroundColor(ConsoleColor color, Action action)
    {
        var oldColor = Console.BackgroundColor;
        Console.BackgroundColor = color;
        action.Invoke();
        Console.BackgroundColor = oldColor;
    }
}
