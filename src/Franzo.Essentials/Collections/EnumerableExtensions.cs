namespace Franzo.Essentials.Collections;

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

    public static TTarget? FirstOrDefault<TSource, TTarget>(
        this IEnumerable<TSource> self,
        Predicate<TTarget>? predicate = null)
    {
        return self.OfType<TTarget>().FirstOrDefault(predicate ?? (_ => true));
    }
}
