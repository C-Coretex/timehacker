using Microsoft.EntityFrameworkCore;
using Timehacker.Integration.Db.Tests;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Infrastructure;
using TimeHacker.Integration.Db.Tests.Fixtures;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

// Every IDbEntity maps PostgreSQL's xmin system column as a concurrency token (no migration). A second
// save against a row that changed underneath must throw DbUpdateConcurrencyException.
public class OptimisticConcurrencyTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("SaveChanges", "OptimisticConcurrency")]
    public async Task ConcurrentUpdate_Should_ThrowDbUpdateConcurrencyException()
    {
        var seeded = await Resolve<IScheduleEntityRepository>().AddAndSaveAsync(
            new ScheduleEntity { RepeatingEntity = GraphSeeder.DailyRepeat() },
            TestContext.Current.CancellationToken);

        var connectionString = Db.Database.GetConnectionString()!;
        await using var context1 = TimeHackerDbContext.Create(connectionString);
        await using var context2 = TimeHackerDbContext.Create(connectionString);

        var entityFromContext1 = await context1.Set<ScheduleEntity>().FirstAsync(x => x.Id == seeded.Id, TestContext.Current.CancellationToken);
        var entityFromContext2 = await context2.Set<ScheduleEntity>().FirstAsync(x => x.Id == seeded.Id, TestContext.Current.CancellationToken);

        // First writer commits, bumping xmin.
        entityFromContext1.EndsOn = new DateOnly(2030, 1, 1);
        await context1.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Second writer still holds the original xmin -> conflict.
        entityFromContext2.EndsOn = new DateOnly(2031, 1, 1);
        var act = async () => await context2.SaveChangesAsync(TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }
}
