using Microsoft.EntityFrameworkCore;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Helpers.DB.Abstractions.BaseClasses
{
    public abstract class ContextFactoryBase<TContext> : IContextFactory<TContext> where TContext : DbContext
    {
        protected string dbConnectionString { get; set; }
        public ContextFactoryBase(string dbConnectionString)
        {
            this.dbConnectionString = dbConnectionString;
        }

        public abstract TContext CreateContext();
    }
}
