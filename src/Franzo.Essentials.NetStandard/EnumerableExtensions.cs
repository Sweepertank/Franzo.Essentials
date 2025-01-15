using System.Collections;

namespace Franzo.Essentials;

public static class EnumerableExtensions
{
    public static IEnumerable<(T, bool)> WithFirstFlag<T>(this IEnumerable<T> self)
    {
        return new WithFirstFlagEnumerable<T>(self);
    }

    public static IEnumerable<(T, bool)> WithLastFlag<T>(this IEnumerable<T> self)
    {
        return new WithLastFlagEnumerable<T>(self);
    }

    public static IEnumerable<(T, bool, bool)> WithFirstAndLastFlags<T>(this IEnumerable<T> self)
    {
        return new WithFirstAndLastFlagsEnumerable<T>(self);
    }

    private class WithFirstFlagEnumerable<T> : IEnumerable<(T, bool)>
    {
        public IEnumerable<T> InnerEnumerable { get; }

        public WithFirstFlagEnumerable(IEnumerable<T> innerEnumerable)
        {
            InnerEnumerable = innerEnumerable;
        }

        public IEnumerator<(T, bool)> GetEnumerator()
        {
            return new WithFirstFlagEnumerator<T>(
                new WithFirstAndLastFlagsEnumerator<T>(InnerEnumerable.GetEnumerator()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private class WithFirstFlagEnumerator<T> : IEnumerator<(T, bool)>
    {
        public WithFirstAndLastFlagsEnumerator<T> InnerEnumerator { get; }

        public (T, bool) Current
        {
            get => (InnerEnumerator.Current.Item1, InnerEnumerator.Current.Item2);
        }

        object IEnumerator.Current
        {
            get => Current;
        }

        public WithFirstFlagEnumerator(WithFirstAndLastFlagsEnumerator<T> innerEnumerator)
        {
            InnerEnumerator = innerEnumerator;
        }

        public bool MoveNext()
        {
            return InnerEnumerator.MoveNext();
        }

        public void Reset()
        {
            InnerEnumerator.Reset();
        }

        public void Dispose()
        {
            InnerEnumerator.Dispose();
        }
    }

    private class WithLastFlagEnumerable<T> : IEnumerable<(T, bool)>
    {
        public IEnumerable<T> InnerEnumerable { get; }

        public WithLastFlagEnumerable(IEnumerable<T> innerEnumerable)
        {
            InnerEnumerable = innerEnumerable;
        }

        public IEnumerator<(T, bool)> GetEnumerator()
        {
            return new WithLastFlagEnumerator<T>(
                new WithFirstAndLastFlagsEnumerator<T>(InnerEnumerable.GetEnumerator()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private class WithLastFlagEnumerator<T> : IEnumerator<(T, bool)>
    {
        public WithFirstAndLastFlagsEnumerator<T> InnerEnumerator { get; }

        public (T, bool) Current
        {
            get => (InnerEnumerator.Current.Item1, InnerEnumerator.Current.Item3);
        }

        object IEnumerator.Current
        {
            get => Current;
        }

        public WithLastFlagEnumerator(WithFirstAndLastFlagsEnumerator<T> innerEnumerator)
        {
            InnerEnumerator = innerEnumerator;
        }

        public bool MoveNext()
        {
            return InnerEnumerator.MoveNext();
        }

        public void Reset()
        {
            InnerEnumerator.Reset();
        }

        public void Dispose()
        {
            InnerEnumerator.Dispose();
        }
    }

    private class WithFirstAndLastFlagsEnumerable<T> : IEnumerable<(T, bool, bool)>
    {
        public IEnumerable<T> InnerEnumerable { get; }

        public WithFirstAndLastFlagsEnumerable(IEnumerable<T> innerEnumerable)
        {
            InnerEnumerable = innerEnumerable;
        }

        public IEnumerator<(T, bool, bool)> GetEnumerator()
        {
            return new WithFirstAndLastFlagsEnumerator<T>(InnerEnumerable.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private class WithFirstAndLastFlagsEnumerator<T> : IEnumerator<(T, bool, bool)>
    {
        private T? _current = default;
        private bool _hasMovedNext = false;

        public IEnumerator<T> InnerEnumerator { get; }
        public bool First { get; private set; } = false;
        public bool Last { get; private set; } = false;

        public (T, bool, bool) Current
        {
            get => _hasMovedNext
                ? (_current!, First, Last)
                : throw new InvalidOperationException("Enumeration already finished.");
        }

        object IEnumerator.Current
        {
            get => Current;
        }

        public WithFirstAndLastFlagsEnumerator(IEnumerator<T> innerEnumerator)
        {
            InnerEnumerator = innerEnumerator;
        }

        public bool MoveNext()
        {
            if (Last)
            {
                return false;
            }
            else if (!_hasMovedNext)
            {
                if (!InnerEnumerator.MoveNext())
                {
                    return false;
                }

                _hasMovedNext = true;
                First = true;
            }
            else
            {
                First = false;
            }

            _current = InnerEnumerator.Current;
            Last = !InnerEnumerator.MoveNext();

            return true;
        }

        public void Reset()
        {
            _current = default;
            _hasMovedNext = false;
            First = false;
            Last = false;
            InnerEnumerator.Reset();
        }

        public void Dispose()
        {
            InnerEnumerator.Dispose();
        }
    }
}
