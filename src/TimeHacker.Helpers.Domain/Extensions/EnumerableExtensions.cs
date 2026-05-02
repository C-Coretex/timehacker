namespace TimeHacker.Helpers.Domain.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        foreach (var item in source)
            action(item);
    }
}
