namespace Franzo.Essentials.Collections;

// https://stackoverflow.com/questions/21758074/compare-two-dictionaries-for-equality

public class DictionaryValueEqualComparer<TKey, TValue> :
    IEqualityComparer<IReadOnlyDictionary<TKey, TValue>>
        where TKey : notnull
{
    private IEqualityComparer<TValue> valueComparer;

    public DictionaryValueEqualComparer(IEqualityComparer<TValue>? valueComparer = null)
    {
        this.valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
    }

    public bool Equals(IReadOnlyDictionary<TKey, TValue>? x, IReadOnlyDictionary<TKey, TValue>? y)
    {
        if (x is null) return y is null;
        if (y is null) return x is null;

        if (x.Count != y.Count)
            return false;
        if (x.Keys.Except(y.Keys).Any())
            return false;
        if (y.Keys.Except(x.Keys).Any())
            return false;
        foreach (var pair in x)
            if (!valueComparer.Equals(pair.Value, y[pair.Key]))
                return false;
        return true;
    }

    public int GetHashCode(IReadOnlyDictionary<TKey, TValue> obj)
    {
        throw new NotImplementedException();
    }
}
