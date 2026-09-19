namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

// ScheduleSnapshot carries an alternate key on (UserId, Date); ScheduledTask/ScheduledCategory FK into
// it via that composite key.
public class ScheduleSnapshotConstraintTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    private static readonly DateOnly Date = new(2026, 6, 1);

    [Fact]
    [Trait("AlternateKey", "UserId+Date")]
    public async Task DuplicateUserDate_Should_BeRejected()
    {
        SharedDb.Add(new ScheduleSnapshot { UserId = CurrentUser.UserId, Date = Date });
        await SharedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Clear the tracker so the second insert is validated by the database's unique constraint rather
        // than EF's in-memory alternate-key identity check.
        SharedDb.ChangeTracker.Clear();
        SharedDb.Add(new ScheduleSnapshot { UserId = CurrentUser.UserId, Date = Date });
        var act = async () => await SharedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    [Trait("AlternateKey", "CompositeNotDateOnly")]
    public async Task SameDateDifferentUsers_Should_Coexist()
    {
        SharedDb.Add(new ScheduleSnapshot { UserId = CurrentUser.UserId, Date = Date });
        SharedDb.Add(new ScheduleSnapshot { UserId = OtherUsers.First().UserId, Date = Date });

        await SharedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        var count = await SharedDb.Set<ScheduleSnapshot>().CountAsync(x => x.Date == Date, TestContext.Current.CancellationToken);
        count.Should().Be(2);
    }

    [Fact]
    [Trait("CompositeFK", "ResolvesAgainstAlternateKey")]
    public async Task ScheduledTask_WithMatchingSnapshot_Should_Insert()
    {
        var snapshot = SharedDb.Add(new ScheduleSnapshot { UserId = CurrentUser.UserId, Date = Date }).Entity;
        await SharedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        SharedDb.Add(new ScheduledTask { UserId = CurrentUser.UserId, Date = Date, Name = "t", IsFixed = true, ScheduleSnapshot = snapshot });
        var act = async () => await SharedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    [Trait("CompositeFK", "RequiresParentSnapshot")]
    public async Task ScheduledTask_WithoutMatchingSnapshot_Should_BeRejected()
    {
        // No snapshot for this (UserId, Date) -> the composite FK cannot resolve.
        SharedDb.Add(new ScheduledTask { UserId = CurrentUser.UserId, Date = new DateOnly(2026, 6, 3), Name = "t", IsFixed = true });
        var act = async () => await SharedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<DbUpdateException>();
    }
}
