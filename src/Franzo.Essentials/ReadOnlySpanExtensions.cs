namespace Franzo.Essentials;

public static class ReadOnlySpanExtensions
{
    public static int IndexOf<T>(this ReadOnlySpan<T> self, Predicate<T> predicate)
    {
        for (var i = 0; i < self.Length; i++)
        {
            var el = self[i];
            if (predicate.Invoke(el))
            {
                return i;
            }
        }

        return -1;
    }
}
