namespace Franzo.Essentials.Collections;

public static class DictionaryExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> self,
        TKey key,
        Func<TValue> defaultValueFactory) where TKey : notnull
    {
        if (!self.TryGetValue(key, out var value))
        {
            value = defaultValueFactory.Invoke();
            self[key] = value;
        }

        return value;
    }
}
