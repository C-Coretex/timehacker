#pragma warning disable CA1062 // Validate arguments of public methods

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TimeHacker.Infrastructure.Interceptors;

// Stamps CreatedTimestamp/UpdatedTimestamp on SaveChanges via the change tracker.
// NOTE: does not run for ExecuteUpdateAsync/ExecuteDeleteAsync (those bypass SaveChanges);
// the UpdatedTimestamp for ExecuteUpdate is stamped explicitly in RepositoryBase.UpdateProperty.
public sealed class TimestampInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private void Stamp(DbContext? context)
    {
        if (context is null) return;

        var now = timeProvider.GetUtcNow().UtcDateTime;

        var createdEntries = context.ChangeTracker.Entries<ICreatable>().Where(entry => entry.State == EntityState.Added);
        foreach (var entry in createdEntries)
            entry.Entity.CreatedTimestamp = now;

        var updatedEntries = context.ChangeTracker.Entries<IUpdatable>().Where(entry => entry.State == EntityState.Modified);
        foreach (var entry in updatedEntries)
            entry.Entity.UpdatedTimestamp = now;
    }
}
