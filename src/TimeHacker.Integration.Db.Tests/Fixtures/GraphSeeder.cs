using System.Drawing;
using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Infrastructure;

namespace TimeHacker.Integration.Db.Tests.Fixtures;

/// <summary>
/// Shared seeders for the integration tests: builds the single entities and multi-entity relationship
/// graphs (cascade, constraint, service-flow) that several test classes rely on. Repository-backed seeds
/// get their UserId stamped by the repository; graphs written through <see cref="TimeHackerDbContext"/>
/// directly stamp the current UserId explicitly.
/// </summary>
internal sealed class GraphSeeder(
    IFixedTaskRepository fixedTaskRepository,
    ICategoryRepository categoryRepository,
    IScheduleEntityRepository scheduleEntityRepository,
    IScheduleSnapshotRepository scheduleSnapshotRepository,
    TimeHackerDbContext dbContext,
    UserAccessorBase userAccessor)
{
    private Guid UserId => userAccessor.GetUserIdOrThrowUnauthorized();

    public static RepeatingEntityDto DailyRepeat()
        => new(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(1));

    public async Task<FixedTask> SeedFixedTaskWithSchedule(CancellationToken cancellationToken)
    {
        var scheduleEntity = await scheduleEntityRepository.AddAndSaveAsync(
            new ScheduleEntity { RepeatingEntity = DailyRepeat() }, cancellationToken);

        return await fixedTaskRepository.AddAndSaveAsync(new FixedTask
        {
            Name = "Scheduled fixed task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
            ScheduleEntityId = scheduleEntity.Id
        }, cancellationToken);
    }

    public async Task<Category> SeedCategoryWithSchedule(CancellationToken cancellationToken)
    {
        var scheduleEntity = await scheduleEntityRepository.AddAndSaveAsync(
            new ScheduleEntity { RepeatingEntity = DailyRepeat() }, cancellationToken);

        return await categoryRepository.AddAndSaveAsync(new Category
        {
            Name = "Scheduled category",
            Color = Color.SteelBlue,
            ScheduleEntityId = scheduleEntity.Id
        }, cancellationToken);
    }

    public Task<ScheduleSnapshot> SeedSnapshotWithChildren(DateOnly date, CancellationToken cancellationToken)
    {
        var snapshot = new ScheduleSnapshot
        {
            Date = date,
            ScheduledTasks =
            {
                new ScheduledTask { Date = date, IsFixed = true, Name = "Scheduled task" }
            },
            ScheduledCategories =
            {
                new ScheduledCategory { Date = date, Name = "Scheduled category", Color = Color.Coral }
            }
        };

        return scheduleSnapshotRepository.AddAndSaveAsync(snapshot, cancellationToken);
    }

    /// <summary>
    /// A FixedTask linked to a Category and a Tag through their junction rows (Category/Tag survive a
    /// task delete; the junctions do not).
    /// </summary>
    public async Task<(Category Category, Tag Tag, FixedTask Task)> SeedFixedTaskWithCategoryAndTagJunctions(CancellationToken cancellationToken)
    {
        var userId = UserId;
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
        dbContext.AddRange(category, tag, task);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Add(new CategoryFixedTask { CategoryId = category.Id, FixedTaskId = task.Id });
        dbContext.Add(new TagFixedTask { TagId = tag.Id, TaskId = task.Id });
        await dbContext.SaveChangesAsync(cancellationToken);

        return (category, tag, task);
    }

    /// <summary>
    /// A ScheduleEntity plus a ScheduleSnapshot whose scheduled children point back at that entity
    /// (deleting the entity clears the children but leaves the snapshot).
    /// </summary>
    public async Task<(ScheduleEntity Schedule, ScheduleSnapshot Snapshot)> SeedScheduleEntityWithSnapshotChildren(DateOnly date, CancellationToken cancellationToken)
    {
        var userId = UserId;
        var scheduleEntity = new ScheduleEntity { UserId = userId, RepeatingEntity = DailyRepeat() };
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
        dbContext.AddRange(scheduleEntity, snapshot);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (scheduleEntity, snapshot);
    }

    /// <summary>
    /// A ScheduleSnapshot containing a single scheduled-task instance generated from the given schedule
    /// entity (used to prove the instance is cascaded when its schedule is deleted).
    /// </summary>
    public async Task<ScheduleSnapshot> SeedSnapshotWithScheduledInstanceFor(Guid scheduleEntityId, DateOnly date, CancellationToken cancellationToken)
    {
        var userId = UserId;
        var snapshot = new ScheduleSnapshot
        {
            UserId = userId,
            Date = date,
            ScheduledTasks =
            {
                new ScheduledTask { UserId = userId, Date = date, Name = "instance", IsFixed = true, ParentScheduleEntityId = scheduleEntityId }
            }
        };
        dbContext.Add(snapshot);
        await dbContext.SaveChangesAsync(cancellationToken);

        return snapshot;
    }

    /// <summary>An unrelated FixedTask that must survive operations targeting other tasks.</summary>
    public Task<FixedTask> SeedUnrelatedFixedTask(CancellationToken cancellationToken)
        => fixedTaskRepository.AddAndSaveAsync(new FixedTask
        {
            Name = "Unrelated",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)
        }, cancellationToken);

    /// <summary>A FixedTask scheduled on a given day at 09:00-10:00 UTC.</summary>
    public Task<FixedTask> SeedFixedTaskOn(DateOnly date, string name, CancellationToken cancellationToken)
        => fixedTaskRepository.AddAndSaveAsync(new FixedTask
        {
            Name = name,
            Priority = 1,
            StartTimestamp = date.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc),
            EndTimestamp = date.ToDateTime(new TimeOnly(10, 0), DateTimeKind.Utc)
        }, cancellationToken);

    /// <summary>
    /// Builds (without persisting) a current-user FixedTask graph whose ScheduleEntity child is a stub
    /// carrying the given id - used to attach/mark a foreign schedule row for RLS update/delete tests.
    /// </summary>
    public static FixedTask BuildTaskWithScheduleStub(Guid scheduleEntityId, Guid userId) => new()
    {
        UserId = userId,
        Name = "Task",
        Priority = 1,
        StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
        EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
        ScheduleEntity = new ScheduleEntity
        {
            Id = scheduleEntityId,
            UserId = userId,
            RepeatingEntity = DailyRepeat()
        }
    };
}
