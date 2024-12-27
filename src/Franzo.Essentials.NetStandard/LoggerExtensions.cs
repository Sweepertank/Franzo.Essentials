using Microsoft.Extensions.Logging;

namespace Franzo.Essentials;

public static class LoggerExtensions
{
    public static ILogger OrNullLogger(this ILogger? self)
    {
        return self ?? NullLogger.Instance;
    }
}
