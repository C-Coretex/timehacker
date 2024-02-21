using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Domain.Abstractions.Interfaces;
using Helpers.Domain.Abstractions.Interfaces.IGenericServices;

namespace Helpers.DB.Abstractions.Classes.GenericServices
{
    public class ServiceQueryBase<TModel> : ServiceAccessorBase<TModel>, IServiceQueryBase<TModel> where TModel : class, IModel
    {
        public ServiceQueryBase(DbSet<TModel> dbSet) : base(dbSet) { }

        public virtual IQueryable<TModel> GetAll()
        {
            return dbSet.AsNoTracking();
        }

        public virtual TModel? GetById(int id)
        {
            return dbSet.AsNoTracking().FirstOrDefault(x => x.Id.Equals(id));
        }

        public virtual async Task<TModel?> GetByIdAsync(int id)
        {
            return await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id));
        }
    }
}
