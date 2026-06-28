using Microsoft.EntityFrameworkCore;
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
    public override TModel Add(TModel model)
    {
        var userId = userAccessor.GetUserIdOrThrowUnauthorized();
        model.UserId = userId;

        return base.Add(model);
    }

    public override async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = userAccessor.GetUserIdOrThrowUnauthorized();

        foreach (var entry in DbContext.ChangeTracker.Entries<IUserScopedEntity>()
                     .Where(e => e.State == EntityState.Added))
            entry.Entity.UserId = userId;

        // Update/Delete operations are handled by RLS: a cross-user mutation hits 0 rows because the
        // row is invisible, which EF (with the xmin token) surfaces as a concurrency conflict. Map it
        // to NotFound for the current user.
        try
        {
            await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Count > 0 ? ex.Entries[0] : null;
            var id = entry?.Metadata.FindPrimaryKey() is { } primaryKey
                ? entry.Property(primaryKey.Properties[0].Name).CurrentValue
                : null;

            throw new NotFoundException(entry?.Metadata.ClrType.Name ?? typeof(TModel).Name,
                id?.ToString() ?? string.Empty);
        }
    }
}
