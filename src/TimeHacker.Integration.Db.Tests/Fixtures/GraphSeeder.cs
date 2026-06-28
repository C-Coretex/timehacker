using System.Drawing;
using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Integration.Db.Tests.Fixtures;

/// <summary>
/// Seeds the multi-entity relationship graphs that several tests (cascade, constraint, service-flow) share
/// </summary>
internal sealed class GraphSeeder(
    IFixedTaskRepository fixedTaskRepository,
    ICategoryRepository categoryRepository,
    IScheduleEntityRepository scheduleEntityRepository,
    IScheduleSnapshotRepository scheduleSnapshotRepository)
{
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
}
