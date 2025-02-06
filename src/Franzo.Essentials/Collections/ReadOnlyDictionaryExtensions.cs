namespace Franzo.Essentials.Collections;

public static class ReadOnlyDictionaryExtensions
{
    public static bool AreValueEqual<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue> a,
        IReadOnlyDictionary<TKey, TValue> b,
        IEqualityComparer<TValue>? valueComparer = null) where TKey : notnull
    {
        return new DictionaryValueEqualComparer<TKey, TValue>(valueComparer).Equals(a, b);
    }

    public static bool IsValueEqualTo<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue>? self,
        IReadOnlyDictionary<TKey, TValue>? other,
        IEqualityComparer<TValue>? valueComparer = null) where TKey : notnull
    {
        if (self is null) return other is null;
        if (other is null) return self is null;

        return AreValueEqual(self, other, valueComparer);
    }
}
