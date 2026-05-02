using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Helpers.Db.Abstractions.BaseClasses;

public abstract class ContextFactoryBase<TContext> : IContextFactory<TContext> where TContext : DbContext
{
    protected string DbConnectionString { get; set; }
    protected ContextFactoryBase(string dbConnectionString)
    {
        DbConnectionString = dbConnectionString;
    }

    public abstract TContext CreateContext();
}
