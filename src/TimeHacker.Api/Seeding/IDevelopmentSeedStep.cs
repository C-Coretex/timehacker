namespace TimeHacker.Api.Seeding;

/// <summary>
/// One self-contained, idempotent unit of development seed data.
/// Implementations must no-op when their data already exists so the whole seed can safely run on every
/// Development startup. Register new steps in <see cref="DevelopmentDataSeeder"/>'s step list.
/// </summary>
internal interface IDevelopmentSeedStep
{
    Task SeedAsync(DevelopmentSeedContext context, CancellationToken cancellationToken);
}
