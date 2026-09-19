namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

/// <summary>
/// Guards the seeding contract the other suites rely on: a seeder must not leave its rows sitting in the
/// change tracker the services under test use. If it does, a tracked read is served from EF's identity map
/// instead of the database, and a test can pass on the seeder's own object graph — which is exactly how a
/// missing Include hides. See <see cref="GraphSeeder"/>.
/// </summary>
public class SeedScopeIsolationTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    [Trait("Seeding", "Seeded rows are not served from the acting scope's change tracker")]
    public async Task ActingScope_Should_ReadSeededRowsFromTheDatabase()
    {
        var seeded = await Seeder.SeedUnrelatedFixedTask(Ct);

        // Change the row behind the acting scope's back. A read served from the identity map would hand
        // back the seeder's instance and never notice.
        await Db.Set<FixedTask>()
            .Where(task => task.Id == seeded.Id)
            .ExecuteUpdateAsync(setter => setter.SetProperty(task => task.Name, "Changed underneath"), Ct);

        var read = await Resolve<IFixedTaskRepository>().GetByIdAsync(seeded.Id, asNoTracking: false, cancellationToken: Ct);

        read!.Name.Should().Be("Changed underneath");
        read.Should().NotBeSameAs(seeded);
    }
}
