using Microsoft.EntityFrameworkCore;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace Helpers.DB.Abstractions.Classes
{
    public abstract class ContextFactoryBase<TContext>: IContextFactory<TContext> where TContext : DbContext
    {
        protected string dbConnectionString { get; set; }
        public ContextFactoryBase(string dbConnectionString)
        {
            this.dbConnectionString = dbConnectionString;
        }

        public abstract TContext CreateContext();
    }
}
