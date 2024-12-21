namespace Franzo.Essentials;

public static class ListExtensions
{
    public static void BinaryInsert<T>(this List<T> self, T item, IComparer<T>? comparer = null)
    {
        var index = self.BinarySearch(item, comparer);
        index = ~index;
        self.Insert(index, item);
    }
}
