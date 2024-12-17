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

    public static int IndexOf<T>(this IReadOnlyList<T> self, T item)
    {
        foreach ((var i, var listItem) in self.IndicesAndItems())
        {
            if (EqualityComparer<T>.Default.Equals(listItem, item))
            {
                return i;
            }
        }

        return -1;
    }
}
