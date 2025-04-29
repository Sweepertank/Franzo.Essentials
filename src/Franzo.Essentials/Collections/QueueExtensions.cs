namespace Franzo.Essentials.Collections;

public static class QueueExtensions
{
    public static void EnqueueRange<T>(this Queue<T> self, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            self.Enqueue(item);
        }
    }
}
