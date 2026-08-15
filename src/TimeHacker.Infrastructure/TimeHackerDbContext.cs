using TimeHacker.Infrastructure.Converters;

namespace TimeHacker.Infrastructure;

public class TimeHackerDbContext : DbContextBase<TimeHackerDbContext>
{
    public TimeHackerDbContext(DbContextOptions<TimeHackerDbContext> options) : base(options) { }
    private TimeHackerDbContext(string connectionString) : base(connectionString) { }
    public static TimeHackerDbContext Create(string connectionString) => new(connectionString);

    // Set per lease by TimeHackerScopedDbContextFactory to the current request/test scope's provider.
    // UserSessionInterceptor resolves UserAccessorBase from it lazily at connection-open (see that class).
    // Overwritten on every lease and nulled on dispose/return-to-pool (below), so a pooled context never
    // holds a reference to a disposed scope.
    internal IServiceProvider? ScopeServiceProvider { get; set; }

    // For a pooled context, disposing the leased instance at end-of-scope is the "return to pool" event.
    // Null the captured scope here so a context sitting in the pool can't read a disposed provider; if it is
    // ever used before the factory re-stamps it.
    public override void Dispose()
    {
        ScopeServiceProvider = null;
        base.Dispose();

        GC.SuppressFinalize(this);
    }

    public override async ValueTask DisposeAsync()
    {
        ScopeServiceProvider = null;
        await base.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    #region DbSets

    //Categories
    internal DbSet<Category> Category { get; set; }
    internal DbSet<CategoryFixedTask> CategoryFixedTask { get; set; }
    internal DbSet<CategoryDynamicTask> CategoryDynamicTask { get; set; }

    //ScheduleSnapshots
    internal DbSet<ScheduledTask> ScheduledTask { get; set; }
    internal DbSet<ScheduledCategory> ScheduledCategory { get; set; }
    internal DbSet<ScheduleSnapshot> ScheduleSnapshot { get; set; }
    internal DbSet<ScheduleEntity> ScheduleEntity { get; set; }

    //Tasks
    internal DbSet<FixedTask> FixedTask { get; set; }
    internal DbSet<DynamicTask> DynamicTask { get; set; }

    //Tags
    internal DbSet<Tag> Tag { get; set; }
    internal DbSet<TagFixedTask> TagFixedTask { get; set; }
    internal DbSet<TagDynamicTask> TagDynamicTask { get; set; }

    //Users
    internal DbSet<User> User { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder);

        // Applies all configurations defined in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        // Optimistic concurrency: map PostgreSQL's system column `xmin` (a system column) as a concurrency
        // token for every domain entity. A tracked update whose row changed concurrently throws DbUpdateConcurrencyException.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(IDbEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            // Equivalent of Npgsql's UseXminAsConcurrencyToken(): map the system column `xmin`
            // as a row-version concurrency token (shadow property, no schema migration needed).
            modelBuilder.Entity(entityType.ClrType)
                .Property<uint>("xmin")
                .HasColumnType("xid")
                .IsRowVersion();
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        ArgumentNullException.ThrowIfNull(configurationBuilder);

        // Model-wide convention: every DateTime is stored/read as UTC via DateTimeUtcConverter.
        configurationBuilder
            .Properties<DateTime>()
            .HaveConversion<DateTimeUtcConverter>();
    }
}
