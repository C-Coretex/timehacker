using Microsoft.EntityFrameworkCore.ChangeTracking;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.IRepositories;

namespace TimeHacker.Infrastructure.Repositories;

internal class UserScopedRepositoryBase<TModel, TId>(TimeHackerDbContext dbContext, DbSet<TModel> dbSet, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : RepositoryBase<TimeHackerDbContext, TModel, TId>(dbContext, dbSet, timeProvider), IUserScopedRepositoryBase<TModel, TId>
    where TModel : class, IDbEntity<TId>, IUserScopedEntity
{
    // The query would be filtered automatically by RLS
    // but we add this for the defence in depth in case if RLS fails
    protected override IQueryable<TModel> GetAllBase()
    {
        var userId = userAccessor.GetUserIdOrThrowUnauthorized();

        var query = base.GetAllBase();
        return query.Where(x => x.UserId == userId);
    }

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

        // Update/Delete operations are handled by RLS, but we implement this for defense in depth
        var modifiedEntries = DbContext.ChangeTracker.Entries<IUserScopedEntity>()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        foreach(var entry in modifiedEntries)
        {
            var originalUserId = entry.Property(e => e.UserId).OriginalValue;

            // OriginalValue could be default if the entity was not loaded, we don't want to throw in that case
            // (this would limit the developer and RLS would handle such case anyway + stubs throw with Optimistic Concurrency)
            originalUserId = originalUserId == Guid.Empty ? entry.Entity.UserId : originalUserId;
            if (originalUserId != entry.Entity.UserId || entry.Entity.UserId != userId)
                ThrowNotFoundException(entry);
        }

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
            ThrowNotFoundException(entry);
        }
    }

    private static void ThrowNotFoundException(EntityEntry? entry)
    {
        var id = entry?.Metadata.FindPrimaryKey() is { } primaryKey
            ? entry.Property(primaryKey.Properties[0].Name).CurrentValue
            : null;
        throw new NotFoundException(entry?.Metadata.ClrType.Name ?? typeof(TModel).Name, id?.ToString() ?? string.Empty);
    }
}
