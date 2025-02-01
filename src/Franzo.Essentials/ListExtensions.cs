namespace Franzo.Essentials;

public static class ListExtensions
{
    public static void BinaryInsert<T>(this List<T> self, T item, IComparer<T>? comparer = null)
    {
        var index = self.BinarySearch(item, comparer);
        if (index < 0)
        {
            index = ~index;
        }

        self.Insert(index, item);
    }

    public static void Truncate<T>(this IList<T> self, int count)
    {
        while (self.Count > count)
        {
            self.RemoveAt(self.Count - 1);
        }
    }
}
