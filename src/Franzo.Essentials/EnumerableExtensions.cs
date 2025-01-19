namespace Franzo.Essentials;

public static class EnumerableExtensions
{
    public static bool NullableSequenceEqual<T>(
        this IEnumerable<T>? self,
        IEnumerable<T>? other,
        IEqualityComparer<T>? equalityComparer = null)
    {
        if (self is null) return other is null;
        if (other is null) return self is null;

        return self.SequenceEqual(other, equalityComparer);
    }
}
