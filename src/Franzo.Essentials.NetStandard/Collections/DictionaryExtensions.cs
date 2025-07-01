namespace Franzo.Essentials.Collections;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> self,
        TKey key,
        Func<TValue> valueFactory) where TKey : notnull
    {
        if (!self.TryGetValue(key, out var value))
        {
            value = valueFactory.Invoke();
            self[key] = value;
        }

        return value;
    }
}
