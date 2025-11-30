namespace TimeHacker.Helpers.Domain.Extensions;

public static class CollectionExtensions
{
    public static T? FirstOrNull<T>(this IEnumerable<T> sequence, Func<T, bool> predicate) where T : struct
    {
        foreach (var item in sequence.Where(predicate))
            return item;

        return null;
    }
}
