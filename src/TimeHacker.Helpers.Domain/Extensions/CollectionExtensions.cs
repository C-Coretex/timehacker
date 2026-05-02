namespace TimeHacker.Helpers.Domain.Extensions;

public static class CollectionExtensions
{
    public static T? FirstOrNull<T>(this IEnumerable<T> sequence, Func<T, bool> predicate) where T : struct
    {
        foreach (var item in sequence.Where(predicate))
            return item;

        return null;
    }

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));
        ArgumentNullException.ThrowIfNull(items, nameof(items));

        foreach (var item in items)
            collection.Add(item);
    }
}
