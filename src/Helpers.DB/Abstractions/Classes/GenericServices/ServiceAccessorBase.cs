using Microsoft.EntityFrameworkCore;
using Helpers.Domain.Abstractions.Interfaces;

namespace Helpers.DB.Abstractions.Classes.GenericServices
{
    public class ServiceAccessorBase<TModel> where TModel : class, IModel
    {
        protected DbSet<TModel> dbSet;
        public ServiceAccessorBase(DbSet<TModel> dbSet)
        {
            this.dbSet = dbSet;
        }
    }
}
