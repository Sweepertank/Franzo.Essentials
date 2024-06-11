namespace Franzo.Essentials;

public static class ReadOnlyListExtensions
{
    public static IEnumerable<(int, T)> IndicesAndItems<T>(this IReadOnlyList<T> self)
    {
        for (var i = 0; i < self.Count; i++)
        {
            yield return (i, self[i]);
        }
    }
}
