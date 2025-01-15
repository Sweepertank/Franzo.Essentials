namespace Franzo.Essentials;

public static class ReadOnlyDictionaryExtensions
{
    public static bool AreValueEqual<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue> a,
        IReadOnlyDictionary<TKey, TValue> b) where TKey : notnull
    {
        return new DictionaryValueEqualComparer<TKey, TValue>().Equals(a, b);
    }
}
