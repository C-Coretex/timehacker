using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Infrastructure.Repositories;

public class UserScopedRepositoryBase<TModel, TId> : RepositoryBase<TimeHackerDbContext, TModel, TId>, IUserScopedRepositoryBase<TModel, TId>
    where TModel : class, IDbEntity<TId>, IUserScopedEntity
{
    private readonly UserAccessorBase _userAccessor;

    public UserScopedRepositoryBase(TimeHackerDbContext dbContext, DbSet<TModel> dbSet, UserAccessorBase userAccessor) : base(dbContext, dbSet)
    {
        _userAccessor = userAccessor;
    }

    protected override IQueryable<TModel> GetAllBase()
    {
        var userId = _userAccessor.GetUserIdOrThrowUnauthorized();
        return base.GetAllBase().Where(x => x.UserId == userId);
    }

    public override TModel Add(TModel model)
    {
        model.UserId = _userAccessor.GetUserIdOrThrowUnauthorized();
        return base.Add(model);
    }

    public override void AddRange(IEnumerable<TModel> models)
    {
        foreach (var model in models)
            Add(model);
    }

    public async Task Delete(TModel model, CancellationToken cancellationToken = default)
    {
        var userId = _userAccessor.GetUserIdOrThrowUnauthorized();
        if (model.UserId != userId)
            return;

        var entityExistsForThisUser = await ExistsAsync(model.Id, cancellationToken);
        if(!entityExistsForThisUser)
            return;

        base.Delete(model);
    }

    public async Task DeleteRange(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        foreach (var model in models)
            await Delete(model, cancellationToken);
    }

    public async Task<TModel> Update(TModel model, CancellationToken cancellationToken = default)
    {
        var userId = _userAccessor.GetUserIdOrThrowUnauthorized();
        if (model.UserId != userId)
            return model;

        var entityExistsForThisUser = await ExistsAsync(model.Id, cancellationToken);
        if (!entityExistsForThisUser)
            return model;

        return base.Update(model);
    }

    public async Task UpdateRange(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        foreach (var model in models)
            await Update(model, cancellationToken);
    }
}
