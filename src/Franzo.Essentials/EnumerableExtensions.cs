namespace Franzo.Essentials;

public static class EnumerableExtensions
{
    public static bool NullableSequenceEquals<T>(
        this IEnumerable<T>? self,
        IEnumerable<T>? other)
    {
        if (self is null) return other is null;
        if (other is null) return self is null;

        return self.SequenceEqual(other);
    }
}
