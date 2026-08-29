using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tags;

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
    ITagRepository tagRepository,
    IScheduleSnapshotRepository scheduleSnapshotRepository,
    IScheduleEntityAppService scheduleEntityAppService,
    TimeHackerDbContext dbContext,
    UserAccessorBase userAccessor)
{
    private Guid UserId => userAccessor.GetUserIdOrThrowUnauthorized();

    public static RepeatingEntityDto DailyRepeat()
        => new(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(1));

    private async Task<TParent> AttachDailySchedule<TParent>(ScheduleEntityParentType parentType, Guid parentId, CancellationToken cancellationToken)
        where TParent : class
    {
        await scheduleEntityAppService.Save(new ScheduleEntityCreateDto(parentType, parentId, DailyRepeat()), cancellationToken);

        dbContext.ChangeTracker.Clear();
        return await dbContext.Set<TParent>().FindAsync([parentId], cancellationToken)
               ?? throw new InvalidOperationException($"Seeded {typeof(TParent).Name} {parentId} disappeared.");
    }

    public async Task<FixedTask> SeedFixedTaskWithSchedule(CancellationToken cancellationToken, DateOnly? on = null)
    {
        var date = on ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var task = await fixedTaskRepository.AddAndSaveAsync(new FixedTask
        {
            Name = "Scheduled fixed task",
            Priority = 1,
            StartTimestamp = date.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc),
            EndTimestamp = date.ToDateTime(new TimeOnly(10, 0), DateTimeKind.Utc)
        }, cancellationToken);

        return await AttachDailySchedule<FixedTask>(ScheduleEntityParentType.FixedTask, task.Id, cancellationToken);
    }

    public async Task<Category> SeedCategoryWithSchedule(CancellationToken cancellationToken, DateOnly? on = null)
    {
        var date = on ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var category = await categoryRepository.AddAndSaveAsync(new Category
        {
            Name = "Scheduled category",
            Color = Color.SteelBlue,
            Date = date,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(10, 0)
        }, cancellationToken);

        return await AttachDailySchedule<Category>(ScheduleEntityParentType.Category, category.Id, cancellationToken);
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
    /// task delete; the junctions do not). The three entities go through their repositories so UserId is
    /// stamped for them; the junctions are written directly because nothing above the DbContext creates
    /// them — <c>FixedTaskDto.GetEntity</c> maps only scalars.
    /// </summary>
    public async Task<(Category Category, Tag Tag, FixedTask Task)> SeedFixedTaskWithCategoryAndTagJunctions(CancellationToken cancellationToken)
    {
        var date = new DateOnly(2026, 6, 1);
        var category = await categoryRepository.AddAndSaveAsync(
            new Category { Name = "Cat", Color = Color.Olive, Date = date, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0) },
            cancellationToken);
        var tag = await tagRepository.AddAndSaveAsync(new Tag { Name = "Tag", Color = Color.Olive }, cancellationToken);
        var task = await fixedTaskRepository.AddAndSaveAsync(new FixedTask
        {
            Name = "Task",
            Priority = 1,
            StartTimestamp = date.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc),
            EndTimestamp = date.ToDateTime(new TimeOnly(10, 0), DateTimeKind.Utc)
        }, cancellationToken);

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
        // A bare FK target: this graph is only ever deleted, never expanded, so it needs no anchor.
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
        // A stub carrying a caller-chosen Id, never expanded — so no anchor, and the app service can't
        // build it anyway (it assigns its own Id).
        ScheduleEntity = new ScheduleEntity
        {
            Id = scheduleEntityId,
            UserId = userId,
            RepeatingEntity = DailyRepeat()
        }
    };
}
