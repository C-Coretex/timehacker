using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Timehacker.Integration.Db.Tests;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Integration.Db.Tests.Fixtures;

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
        var date = new DateOnly(2026, 6, 2);
        var userId = CurrentUser.UserId;
        var scheduleEntity = new ScheduleEntity { UserId = userId, RepeatingEntity = GraphSeeder.DailyRepeat() };
        Db.Add(scheduleEntity);

        var snapshot = new ScheduleSnapshot
        {
            UserId = userId,
            Date = date,
            ScheduledTasks =
            {
                new ScheduledTask { UserId = userId, Date = date, Name = "t", IsFixed = true, ParentScheduleEntityId = scheduleEntity.Id }
            },
            ScheduledCategories =
            {
                new ScheduledCategory { UserId = userId, Date = date, Name = "c", Color = Color.Coral, ParentScheduleEntity = scheduleEntity.Id }
            }
        };
        Db.Add(snapshot);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        await DeleteByIdAsync<ScheduleEntity>(scheduleEntity.Id);

        (await ExistsAsync<ScheduleSnapshot>(snapshot.Id)).Should().BeTrue();
        (await CountAsync<ScheduledTask>()).Should().Be(0);
        (await CountAsync<ScheduledCategory>()).Should().Be(0);
    }

    [Fact]
    [Trait("Cascade", "FixedTask->Junctions")]
    public async Task DeletingFixedTask_Should_RemoveJunctionsButKeepCategoryAndTag()
    {
        var (category, tag, task) = await SeedTaskWithJunctions();

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
        var (category, _, task) = await SeedTaskWithJunctions();

        await DeleteByIdAsync<Category>(category.Id);

        (await CountAsync<CategoryFixedTask>()).Should().Be(0);
        (await ExistsAsync<FixedTask>(task.Id)).Should().BeTrue();
    }

    private async Task<(Category Category, Tag Tag, FixedTask Task)> SeedTaskWithJunctions()
    {
        var userId = CurrentUser.UserId;
        var category = new Category { UserId = userId, Name = "Cat", Color = Color.Olive };
        var tag = new Tag { UserId = userId, Name = "Tag", Color = Color.Olive };
        var task = new FixedTask
        {
            UserId = userId,
            Name = "Task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)
        };
        Db.AddRange(category, tag, task);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        Db.Add(new CategoryFixedTask { CategoryId = category.Id, FixedTaskId = task.Id });
        Db.Add(new TagFixedTask { TagId = tag.Id, TaskId = task.Id });
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (category, tag, task);
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
