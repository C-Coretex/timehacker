using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Infrastructure.Repositories;

internal class UserScopedRepositoryBase<TModel, TId> : RepositoryBase<TimeHackerDbContext, TModel, TId>, IUserScopedRepositoryBase<TModel, TId>
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
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        model.UserId = _userAccessor.GetUserIdOrThrowUnauthorized();
        return base.Add(model);
    }

    public override void AddRange(IEnumerable<TModel> models)
    {
        ArgumentNullException.ThrowIfNull(models, nameof(models));

        foreach (var model in models)
            Add(model);
    }

    public async Task Delete(TModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        var userId = _userAccessor.GetUserIdOrThrowUnauthorized();
        if (model.UserId != userId)
            throw new NotFoundException(typeof(TModel).Name, model.Id?.ToString() ?? string.Empty);

        var entityExistsForThisUser = await ExistsAsync(model.Id, cancellationToken);
        if (!entityExistsForThisUser)
            throw new NotFoundException(typeof(TModel).Name, model.Id?.ToString() ?? string.Empty);

        base.Delete(model);
    }

    public async Task DeleteRange(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(models, nameof(models));

        foreach (var model in models)
            await Delete(model, cancellationToken);
    }

    public async Task<TModel> Update(TModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        var userId = _userAccessor.GetUserIdOrThrowUnauthorized();
        if (model.UserId != userId)
            throw new NotFoundException(typeof(TModel).Name, model.Id?.ToString() ?? string.Empty);

        var entityExistsForThisUser = await ExistsAsync(model.Id, cancellationToken);
        if (!entityExistsForThisUser)
            throw new NotFoundException(typeof(TModel).Name, model.Id?.ToString() ?? string.Empty);

        return base.Update(model);
    }

    public async Task UpdateRange(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(models, nameof(models));

        foreach (var model in models)
            await Update(model, cancellationToken);
    }
}
