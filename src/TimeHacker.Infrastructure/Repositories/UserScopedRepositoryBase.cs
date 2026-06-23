using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Infrastructure.Repositories;

internal class UserScopedRepositoryBase<TModel, TId>(TimeHackerDbContext dbContext, DbSet<TModel> dbSet, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : RepositoryBase<TimeHackerDbContext, TModel, TId>(dbContext, dbSet, timeProvider), IUserScopedRepositoryBase<TModel, TId>
    where TModel : class, IDbEntity<TId>, IUserScopedEntity
{
    protected override IQueryable<TModel> GetAllBase()
    {
        var userId = userAccessor.GetUserIdOrThrowUnauthorized();
        return base.GetAllBase().Where(x => x.UserId == userId);
    }

    public override TModel Add(TModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.UserId = userAccessor.GetUserIdOrThrowUnauthorized();
        return base.Add(model);
    }

    public override void AddRange(IEnumerable<TModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        foreach (var model in models)
            Add(model);
    }

    public async Task Delete(TModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var userId = userAccessor.GetUserIdOrThrowUnauthorized();
        if (model.UserId != userId)
            throw new NotFoundException(typeof(TModel).Name, model.Id?.ToString() ?? string.Empty);

        var entityExistsForThisUser = await ExistsAsync(model.Id, cancellationToken);
        if (!entityExistsForThisUser)
            throw new NotFoundException(typeof(TModel).Name, model.Id?.ToString() ?? string.Empty);

        base.Delete(model);
    }

    public async Task DeleteRange(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(models);

        foreach (var model in models)
            await Delete(model, cancellationToken);
    }

    public async Task<TModel> Update(TModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var userId = userAccessor.GetUserIdOrThrowUnauthorized();
        if (model.UserId != userId)
            throw new NotFoundException(typeof(TModel).Name, model.Id?.ToString() ?? string.Empty);

        var entityExistsForThisUser = await ExistsAsync(model.Id, cancellationToken);
        if (!entityExistsForThisUser)
            throw new NotFoundException(typeof(TModel).Name, model.Id?.ToString() ?? string.Empty);

        return base.Update(model);
    }

    public async Task UpdateRange(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(models);

        foreach (var model in models)
            await Update(model, cancellationToken);
    }
}
