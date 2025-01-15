namespace Franzo.Essentials;

public static class PredicateHelper
{
    public static Predicate<T> Tautology<T>()
    {
        return x => true;
    }

    public static Predicate<T> OrTautology<T>(this Predicate<T>? self)
    {
        return self ?? Tautology<T>();
    }

    public static Predicate<T> Contradiction<T>()
    {
        return x => false;
    }

    public static Predicate<T> OrContradiction<T>(this Predicate<T>? self)
    {
        return self ?? Contradiction<T>();
    }
}
