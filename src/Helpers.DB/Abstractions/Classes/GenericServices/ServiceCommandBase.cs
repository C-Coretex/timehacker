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
    public class ServiceCommandBase<TDbContext, TModel> : ServiceAccessorBase<TModel>, IServiceCommandBase<TModel> where TModel : class, IModel, new()
                                                                                                                  where TDbContext : DbContextBase<TDbContext>
    {
        protected TDbContext dbContext;
        public ServiceCommandBase(TDbContext dbContext, DbSet<TModel> dbSet) : base(dbSet)
        {
            this.dbContext = dbContext;
        }

        public virtual TModel Add(TModel model, bool saveChanges = true)
        {
            return dbContext.AddEntity(dbSet, model, saveChanges);
        }
        public virtual Task<TModel> AddAsync(TModel model, bool saveChanges = true)
        {
            return dbContext.AddEntityAsync(dbSet, model, saveChanges);
        }

        public virtual IEnumerable<TModel> AddRange(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.AddEntities(dbSet, models, saveChanges);
        }
        public virtual Task<IEnumerable<TModel>> AddRangeAsync(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.AddEntitiesAsync(dbSet, models, saveChanges);
        }

        public virtual void Delete(TModel model, bool saveChanges = true)
        {
            dbContext.RemoveEntity(dbSet, model, saveChanges);
        }
        public virtual Task DeleteAsync(TModel model, bool saveChanges = true)
        {
            return dbContext.RemoveEntityAsync(dbSet, model, saveChanges);
        }

        public virtual void DeleteRange(IEnumerable<TModel> models, bool saveChanges = true)
        {
            dbContext.RemoveEntities(dbSet, models, saveChanges);
        }
        public virtual Task DeleteRangeAsync(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.RemoveEntitiesAsync(dbSet, models, saveChanges);
        }

        public virtual void Delete(int id, bool saveChanges = true)
        {
            dbContext.RemoveEntity(dbSet, id, saveChanges);
        }
        public virtual Task DeleteAsync(int id, bool saveChanges = true)
        {
            return dbContext.RemoveEntityAsync(dbSet, id, saveChanges);
        }

        public virtual void DeleteRange(IEnumerable<int> ids, bool saveChanges = true)
        {
            dbContext.RemoveEntities(dbSet, ids, saveChanges);
        }
        public virtual Task DeleteRangeAsync(IEnumerable<int> ids, bool saveChanges = true)
        {
            return dbContext.RemoveEntitiesAsync(dbSet, ids, saveChanges);
        }

        public virtual TModel Update(TModel model, bool saveChanges = true)
        {
            return dbContext.UpdateEntity(dbSet, model, saveChanges);
        }
        public virtual Task<TModel> UpdateAsync(TModel model, bool saveChanges = true)
        {
            return dbContext.UpdateEntityAsync(dbSet, model, saveChanges);
        }

        public virtual IEnumerable<TModel> UpdateRange(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.UpdateEntities(dbSet, models, saveChanges);
        }
        public virtual Task<IEnumerable<TModel>> UpdateRangeAsync(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.UpdateEntitiesAsync(dbSet, models, saveChanges);
        }

        public virtual void SaveChanges()
        {
            dbContext.SaveDBChanges();
        }

        public virtual Task SaveChangesAsync(CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;
            return dbContext.SaveDBChangesAsync(cancellationToken.Value);
        }
    }
}
