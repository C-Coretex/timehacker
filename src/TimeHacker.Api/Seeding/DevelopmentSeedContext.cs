using TimeHacker.Infrastructure;

namespace TimeHacker.Api.Seeding;

/// <summary>
/// Everything a <see cref="IDevelopmentSeedStep"/> needs to seed one concern for the dev user.
/// <see cref="Db"/> is the raw admin context (bypasses RLS, has no interceptors), so steps must stamp
/// <c>UserId</c> and <c>CreatedTimestamp</c> on every row themselves. <see cref="Today"/>/<see cref="Now"/>
/// let steps place data relative to the current date instead of hardcoding it.
/// </summary>
internal record DevelopmentSeedContext
{
    public required TimeHackerDbContext Db { get; init; }
    public required Guid UserId { get; init; }
    public required DateOnly Today { get; init; }
    public required DateTime Now { get; init; }
}
