namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

// Cascade-delete behavior is declared in EF config and enforced by PostgreSQL; only a real database
// proves the chains fire correctly.
public class CascadeDeleteTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("Cascade", "ScheduleEntity->FixedTask")]
    public async Task DeletingScheduleEntity_Should_CascadeDeleteFixedTask()
    {
        var task = await Resolve<GraphSeeder>().SeedFixedTaskWithSchedule(TestContext.Current.CancellationToken);

        await DeleteByIdAsync<ScheduleEntity>(task.ScheduleEntityId!.Value);

        (await ExistsAsync<FixedTask>(task.Id)).Should().BeFalse();
    }

    [Fact]
    [Trait("Cascade", "ScheduleEntity->Category")]
    public async Task DeletingScheduleEntity_Should_CascadeDeleteCategory()
    {
        var category = await Resolve<GraphSeeder>().SeedCategoryWithSchedule(TestContext.Current.CancellationToken);

        await DeleteByIdAsync<ScheduleEntity>(category.ScheduleEntityId!.Value);

        (await ExistsAsync<Category>(category.Id)).Should().BeFalse();
    }

    [Fact]
    [Trait("Cascade", "Snapshot->ScheduledChildren")]
    public async Task DeletingSnapshot_Should_CascadeDeleteScheduledChildren()
    {
        var snapshot = await Resolve<GraphSeeder>().SeedSnapshotWithChildren(new DateOnly(2026, 6, 1), TestContext.Current.CancellationToken);

        await DeleteByIdAsync<ScheduleSnapshot>(snapshot.Id);

        (await CountAsync<ScheduledTask>()).Should().Be(0);
        (await CountAsync<ScheduledCategory>()).Should().Be(0);
    }

    [Fact]
    [Trait("Cascade", "ScheduleEntity->ScheduledChildren")]
    public async Task DeletingScheduleEntity_Should_CascadeDeleteScheduledChildrenButKeepSnapshot()
    {
        var (scheduleEntity, snapshot) = await Resolve<GraphSeeder>()
            .SeedScheduleEntityWithSnapshotChildren(new DateOnly(2026, 6, 2), TestContext.Current.CancellationToken);

        await DeleteByIdAsync<ScheduleEntity>(scheduleEntity.Id);

        (await ExistsAsync<ScheduleSnapshot>(snapshot.Id)).Should().BeTrue();
        (await CountAsync<ScheduledTask>()).Should().Be(0);
        (await CountAsync<ScheduledCategory>()).Should().Be(0);
    }

    [Fact]
    [Trait("Cascade", "FixedTask->Junctions")]
    public async Task DeletingFixedTask_Should_RemoveJunctionsButKeepCategoryAndTag()
    {
        var (category, tag, task) = await Resolve<GraphSeeder>().SeedFixedTaskWithCategoryAndTagJunctions(TestContext.Current.CancellationToken);

        await DeleteByIdAsync<FixedTask>(task.Id);

        (await CountAsync<CategoryFixedTask>()).Should().Be(0);
        (await CountAsync<TagFixedTask>()).Should().Be(0);
        (await ExistsAsync<Category>(category.Id)).Should().BeTrue();
        (await ExistsAsync<Tag>(tag.Id)).Should().BeTrue();
    }

    [Fact]
    [Trait("Cascade", "Category->Junction")]
    public async Task DeletingCategory_Should_RemoveJunctionButKeepFixedTask()
    {
        var (category, _, task) = await Resolve<GraphSeeder>().SeedFixedTaskWithCategoryAndTagJunctions(TestContext.Current.CancellationToken);

        await DeleteByIdAsync<Category>(category.Id);

        (await CountAsync<CategoryFixedTask>()).Should().Be(0);
        (await ExistsAsync<FixedTask>(task.Id)).Should().BeTrue();
    }

    private async Task DeleteByIdAsync<TEntity>(Guid id) where TEntity : class
    {
        Db.ChangeTracker.Clear();
        var entity = await Db.Set<TEntity>().FindAsync([id], TestContext.Current.CancellationToken);
        Db.Remove(entity!);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);
        Db.ChangeTracker.Clear();
    }

    private async Task<bool> ExistsAsync<TEntity>(Guid id) where TEntity : class
        => await Db.Set<TEntity>().FindAsync([id], TestContext.Current.CancellationToken) is not null;

    private async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        Db.ChangeTracker.Clear();
        return await Db.Set<TEntity>().CountAsync(TestContext.Current.CancellationToken);
    }
}
