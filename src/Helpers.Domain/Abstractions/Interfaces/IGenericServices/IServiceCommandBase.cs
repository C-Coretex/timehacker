using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Domain.Abstractions.Interfaces.IGenericServices
{
    public interface IServiceCommandBase<TModel> where TModel : IModel
    {
        public TModel Add(TModel model, bool saveChanges = true);
        public Task<TModel> AddAsync(TModel model, bool saveChanges = true);
        public IEnumerable<TModel> AddRange(IEnumerable<TModel> models, bool saveChanges = true);
        public Task<IEnumerable<TModel>> AddRangeAsync(IEnumerable<TModel> models, bool saveChanges = true);
        public  TModel Update(TModel model, bool saveChanges = true);
        public Task<TModel> UpdateAsync(TModel model, bool saveChanges = true);
        public IEnumerable<TModel> UpdateRange(IEnumerable<TModel> models, bool saveChanges = true);
        public Task<IEnumerable<TModel>> UpdateRangeAsync(IEnumerable<TModel> models, bool saveChanges = true);
        public void Delete(TModel model, bool saveChanges = true);
        public Task DeleteAsync(TModel model, bool saveChanges = true);
        public void DeleteRange(IEnumerable<TModel> models, bool saveChanges = true);
        public Task DeleteRangeAsync(IEnumerable<TModel> models, bool saveChanges = true);
        public void Delete(int id, bool saveChanges = true);
        public Task DeleteAsync(int id, bool saveChanges = true);
        public void DeleteRange(IEnumerable<int> ids, bool saveChanges = true);
        public Task DeleteRangeAsync(IEnumerable<int> ids, bool saveChanges = true);
        public void SaveChanges();
        public Task SaveChangesAsync(CancellationToken? cancellationToken);
    }
}
