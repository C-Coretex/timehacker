using Microsoft.AspNetCore.Identity;
using TimeHacker.Api.Seeding.Steps;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Infrastructure;

namespace TimeHacker.Api.Seeding;

/// <summary>
/// Seeds a ready-to-use account and sample data for local development so the frontend's "Quick Dev Login"
/// (<c>test@aa.bb</c> / <c>Qwerty123</c>) works out of the box against a fresh database.
///
/// Called only from the Development branch of <c>Program.cs</c>, after migrations. It is idempotent: the
/// Identity user, the domain user, and every seed step each no-op when their data already exists, so it is
/// safe to run on every startup.
///
/// App-DB rows are written through the <b>admin</b> connection (<see cref="TimeHackerDbContext.Create"/>),
/// which is the DB owner and bypasses Row-Level Security — the normal pooled context would be RLS-blocked at
/// startup because there is no HTTP user context. That raw context has no interceptors, so the seeder and its
/// steps stamp <c>UserId</c>/<c>CreatedTimestamp</c> explicitly.
/// </summary>
internal static class DevelopmentDataSeeder
{
    private const string DevUserEmail = "test@aa.bb";
    private const string DevUserPassword = "Qwerty123";

    private static readonly IReadOnlyList<IDevelopmentSeedStep> Steps =
    [
        new TasksSeedStep()
    ];

    public static async Task SeedAsync(
        IServiceProvider services,
        string adminConnectionString,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var now = scope.ServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().UtcDateTime;

        var identityId = await EnsureIdentityUserAsync(userManager);

        await using var db = TimeHackerDbContext.Create(adminConnectionString);

        var userId = await EnsureDomainUserAsync(db, identityId, now, cancellationToken);

        var context = new DevelopmentSeedContext
        {
            Db = db,
            UserId = userId,
            Today = DateOnly.FromDateTime(now),
            Now = now
        };

        foreach (var step in Steps)
            await step.SeedAsync(context, cancellationToken);
    }

    /// <returns>The Identity user's id (its <c>NameIdentifier</c>), which the domain user links to.</returns>
    private static async Task<string> EnsureIdentityUserAsync(UserManager<IdentityUser> userManager)
    {
        var existing = await userManager.FindByEmailAsync(DevUserEmail);
        if (existing is not null)
            return existing.Id;

        var user = new IdentityUser
        {
            UserName = DevUserEmail,
            Email = DevUserEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, DevUserPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new InvalidOperationException($"Failed to seed development identity user: {errors}");
        }

        return user.Id;
    }

    /// <returns>The domain user's id, used to scope all seeded data.</returns>
    private static async Task<Guid> EnsureDomainUserAsync(
        TimeHackerDbContext db,
        string identityId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var users = db.Set<User>();
        var existing = await users.FirstOrDefaultAsync(u => u.IdentityId == identityId, cancellationToken);
        if (existing is not null)
            return existing.Id;

        var user = new User
        {
            IdentityId = identityId,
            Name = "Test User",
            EmailForNotifications = DevUserEmail,
            CreatedTimestamp = now
        };

        users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
