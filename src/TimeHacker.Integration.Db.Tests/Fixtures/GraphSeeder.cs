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
    IDynamicTaskRepository dynamicTaskRepository,
    ICategoryRepository categoryRepository,
    ICategoryScheduleRepository categoryScheduleRepository,
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

    /// <summary>A Category owning one window, with a daily recurrence attached to that window.</summary>
    public async Task<CategorySchedule> SeedCategoryScheduleWithSchedule(CancellationToken cancellationToken, DateOnly? on = null)
    {
        var date = on ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var category = await categoryRepository.AddAndSaveAsync(new Category
        {
            Name = "Scheduled category",
            Color = Color.SteelBlue
        }, cancellationToken);

        var schedule = await categoryScheduleRepository.AddAndSaveAsync(new CategorySchedule
        {
            CategoryId = category.Id,
            Description = "Scheduled window",
            Date = date,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(10, 0)
        }, cancellationToken);

        return await AttachDailySchedule<CategorySchedule>(ScheduleEntityParentType.CategorySchedule, schedule.Id, cancellationToken);
    }

    /// <summary>
    /// One category plus one window, both owned by whichever user's scope resolved this seeder. Used where
    /// <see cref="SeedDataBuilder{TRepository,TModel,TId}"/> cannot help: AutoFaker's navigation binder skips
    /// <c>CategoryId</c> (it matches the <c>Category</c> navigation), which would violate the foreign key.
    /// </summary>
    public async Task<CategorySchedule> SeedCategoryScheduleForCurrentUser(string? windowDescription = "Working hours", DateOnly? on = null)
    {
        return await AddCategoryScheduleForCurrentUser(windowDescription, on, TestContext.Current.CancellationToken);
    }

    /// <summary>Two windows on the same day under one category — allowed, and free to overlap.</summary>
    public async Task<(Category Category, CategorySchedule Morning, CategorySchedule Evening)> SeedCategoryWithTwoWindowsOn(DateOnly date, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.AddAndSaveAsync(
            new Category { Name = "Work", Color = Color.SteelBlue }, cancellationToken);

        var morning = await categoryScheduleRepository.AddAndSaveAsync(new CategorySchedule
        {
            CategoryId = category.Id,
            Description = "Working hours",
            Date = date,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(12, 0)
        }, cancellationToken);

        var evening = await categoryScheduleRepository.AddAndSaveAsync(new CategorySchedule
        {
            CategoryId = category.Id,
            Description = "Overtime",
            Date = date,
            StartTime = new TimeOnly(18, 0),
            EndTime = new TimeOnly(20, 0)
        }, cancellationToken);

        return (category, morning, evening);
    }

    public async Task<ScheduleSnapshot> SeedSnapshotWithChildren(DateOnly date, CancellationToken cancellationToken)
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
                new ScheduledCategory { Date = date, Name = "Scheduled category", ScheduleDescription = "Scheduled window", Color = Color.Coral }
            }
        };

        var saved = await scheduleSnapshotRepository.AddAndSaveAsync(snapshot, cancellationToken);

        return saved;
    }

    /// <summary>A bare Category owned by the current user — a link target, with no time window of its own.</summary>
    public Task<Category> SeedCategoryForCurrentUser(string name = "Cat", CancellationToken cancellationToken = default)
        => categoryRepository.AddAndSaveAsync(new Category { Name = name, Color = Color.Olive }, cancellationToken);

    private async Task<CategorySchedule> AddCategoryScheduleForCurrentUser(string? windowDescription, DateOnly? on, CancellationToken cancellationToken)
    {
        var category = await SeedCategoryForCurrentUser("Work", cancellationToken);

        return await categoryScheduleRepository.AddAndSaveAsync(new CategorySchedule
        {
            CategoryId = category.Id,
            Description = windowDescription,
            Date = on ?? DateOnly.FromDateTime(DateTime.UtcNow),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(10, 0)
        }, cancellationToken);
    }

    /// <summary>
    /// A FixedTask linked to a Category and a Tag through their junction rows (Category/Tag survive a
    /// task delete; the junctions do not). The three entities go through their repositories so UserId is
    /// stamped for them; the junctions are written directly so the graph exists independently of the
    /// service under test.
    /// </summary>
    public async Task<(Category Category, Tag Tag, FixedTask Task)> SeedFixedTaskWithCategoryAndTagJunctions(CancellationToken cancellationToken)
    {
        var date = new DateOnly(2026, 6, 1);
        var category = await categoryRepository.AddAndSaveAsync(
            new Category { Name = "Cat", Color = Color.Olive },
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

    /// <summary>A task's category with a window of its own, so a read path can be held to loading both hops.</summary>
    public async Task<(CategorySchedule Window, FixedTask Task)> SeedFixedTaskLinkedToScheduledCategory(CancellationToken cancellationToken)
    {
        var window = await AddCategoryScheduleForCurrentUser("Working hours", on: null, cancellationToken);
        var task = await fixedTaskRepository.AddAndSaveAsync(new FixedTask
        {
            Name = "Task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)
        }, cancellationToken);

        dbContext.Add(new CategoryFixedTask { CategoryId = window.CategoryId, FixedTaskId = task.Id });
        await dbContext.SaveChangesAsync(cancellationToken);

        return (window, task);
    }

    /// <summary>The <see cref="SeedFixedTaskLinkedToScheduledCategory"/> graph, for a DynamicTask.</summary>
    public async Task<(CategorySchedule Window, DynamicTask Task)> SeedDynamicTaskLinkedToScheduledCategory(CancellationToken cancellationToken)
    {
        var window = await AddCategoryScheduleForCurrentUser("Working hours", on: null, cancellationToken);
        var task = await dynamicTaskRepository.AddAndSaveAsync(new DynamicTask
        {
            Name = "Task",
            Priority = 1,
            MinTimeToFinish = TimeSpan.FromMinutes(30),
            MaxTimeToFinish = TimeSpan.FromMinutes(60)
        }, cancellationToken);

        dbContext.Add(new CategoryDynamicTask { CategoryId = window.CategoryId, DynamicTaskId = task.Id });
        await dbContext.SaveChangesAsync(cancellationToken);

        return (window, task);
    }

    /// <summary>The <see cref="SeedFixedTaskWithCategoryAndTagJunctions"/> graph, for a DynamicTask.</summary>
    public async Task<(Category Category, Tag Tag, DynamicTask Task)> SeedDynamicTaskWithCategoryAndTagJunctions(CancellationToken cancellationToken)
    {
        var category = await SeedCategoryForCurrentUser("Cat", cancellationToken);
        var tag = await tagRepository.AddAndSaveAsync(new Tag { Name = "Tag", Color = Color.Olive }, cancellationToken);
        var task = await dynamicTaskRepository.AddAndSaveAsync(new DynamicTask
        {
            Name = "Task",
            Priority = 1,
            MinTimeToFinish = TimeSpan.FromMinutes(30),
            MaxTimeToFinish = TimeSpan.FromMinutes(60)
        }, cancellationToken);

        dbContext.Add(new CategoryDynamicTask { CategoryId = category.Id, DynamicTaskId = task.Id });
        dbContext.Add(new TagDynamicTask { TagId = tag.Id, TaskId = task.Id });
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
    public async Task<FixedTask> SeedUnrelatedFixedTask(CancellationToken cancellationToken)
    {
        var task = await fixedTaskRepository.AddAndSaveAsync(new FixedTask
        {
            Name = "Unrelated",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)
        }, cancellationToken);

        return task;
    }

    /// <summary>A FixedTask scheduled on a given day at 09:00-10:00 UTC.</summary>
    public async Task<FixedTask> SeedFixedTaskOn(DateOnly date, string name, CancellationToken cancellationToken)
    {
        var task = await fixedTaskRepository.AddAndSaveAsync(new FixedTask
        {
            Name = name,
            Priority = 1,
            StartTimestamp = date.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc),
            EndTimestamp = date.ToDateTime(new TimeOnly(10, 0), DateTimeKind.Utc)
        }, cancellationToken);

        return task;
    }

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
