using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Helpers.Domain.Abstractions.Interfaces
{
    public interface IRepositoryBase<TModel> where TModel : class, IDbModel, new()
    {
        IQueryable<TModel> GetAll(params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates);
        IQueryable<TModel> GetAll(bool asNoTracking = true, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates);
        TModel Add(TModel model, bool saveChanges = true);
        Task<TModel> AddAsync(TModel model, bool saveChanges = true);
        IEnumerable<TModel> AddRange(IEnumerable<TModel> models, bool saveChanges = true);
        Task<IEnumerable<TModel>> AddRangeAsync(IEnumerable<TModel> models, bool saveChanges = true);
        void Delete(TModel model, bool saveChanges = true);
        Task DeleteAsync(TModel model, bool saveChanges = true);
        void DeleteRange(IEnumerable<TModel> models, bool saveChanges = true);
        Task DeleteRangeAsync(IEnumerable<TModel> models, bool saveChanges = true);
        TModel Update(TModel model, bool saveChanges = true);
        Task<TModel> UpdateAsync(TModel model, bool saveChanges = true);
        IEnumerable<TModel> UpdateRange(IEnumerable<TModel> models, bool saveChanges = true);
        Task<IEnumerable<TModel>> UpdateRangeAsync(IEnumerable<TModel> models, bool saveChanges = true);
        void SaveChanges();
        Task SaveChangesAsync(CancellationToken? cancellationToken = null);
    }

    public interface IRepositoryBase<TModel, TId>: IRepositoryBase<TModel> where TModel : class, IDbModel<TId>, new()
    {
        TModel? GetById(TId id, bool asNoTracking = true, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates);
        Task<TModel?> GetByIdAsync(TId id, bool asNoTracking = true, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates);
        void Delete(TId id, bool saveChanges = true);
        Task DeleteAsync(TId id, bool saveChanges = true);
        void DeleteRange(IEnumerable<TId> ids, bool saveChanges = true);
        Task DeleteRangeAsync(IEnumerable<TId> ids, bool saveChanges = true);
    }
}
