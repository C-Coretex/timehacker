namespace TimeHacker.Helpers.Domain.Abstractions.Delegates
{
    public delegate IQueryable<T> IncludeExpansionDelegate<T>(IQueryable<T> query);
}
