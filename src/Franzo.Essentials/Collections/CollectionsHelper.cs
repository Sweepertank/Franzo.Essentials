namespace Franzo.Essentials.Collections;

public static class CollectionsHelper
{
    public static List<T> CreateSingletonList<T>(T item)
    {
        return CreateOneOrZeroItemList(item);
    }

    public static List<T> CreateOneOrZeroItemList<T>(T? item)
    {
        if (item is null)
        {
            return new List<T>();
        }

        return new List<T>() { item };
    }
}
