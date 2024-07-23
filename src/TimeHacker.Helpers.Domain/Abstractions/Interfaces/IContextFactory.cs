namespace TimeHacker.Helpers.Domain.Abstractions.Interfaces
{
    public interface IContextFactory<TContext> where TContext : class
    {
        public TContext CreateContext();
    }
}
