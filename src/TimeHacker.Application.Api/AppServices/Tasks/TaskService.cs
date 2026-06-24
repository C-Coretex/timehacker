using System.Runtime.CompilerServices;
using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Application.Api.QueryPipelineSteps;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Application.Api.AppServices.Tasks;

public class TaskService(
    IFixedTaskRepository fixedTaskRepository,
    IDynamicTaskRepository dynamicTaskRepository,
    IScheduleSnapshotRepository scheduleSnapshotRepository,
    IScheduleEntityService scheduleEntityService,
    ITaskTimelineProcessor taskTimelineProcessor) : ITaskAppService
{
    // Upper bound on how many days a single timeline request may span / generate.
    private const int MaxTimelineDays = 366;

    public async Task<TasksForDayDto> GetTasksForDay(DateOnly date, CancellationToken cancellationToken = default)
    {
        var snapshot = await GetSnapshotForDate(date, cancellationToken);
        if (snapshot != null)
            return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));

        var fixedTasks = await fixedTaskRepository.GetAll()
                                          .Where(ft => DateOnly.FromDateTime(ft.StartTimestamp) == date)
                                          .OrderBy(ft => ft.StartTimestamp)
                                          .ToListAsync(cancellationToken);

        var dynamicTasks = await dynamicTaskRepository.GetAll().ToListAsync(cancellationToken);

        var scheduledFixedTasks = await GetFixedTasksForScheduledTasks(date, cancellationToken: cancellationToken).ToListAsync(cancellationToken);

        var tasksForDay = taskTimelineProcessor.GetTasksForDay(fixedTasks, scheduledFixedTasks, dynamicTasks, date);

        snapshot = tasksForDay.CreateOrUpdateScheduleSnapshot();
        snapshot = await scheduleSnapshotRepository.AddAndSaveAsync(snapshot, cancellationToken);

        return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
    }

    public async IAsyncEnumerable<TasksForDayDto> GetTasksForDays(ICollection<DateOnly> dates, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (dates == null || dates.Count == 0)
            yield break;

        EnsureDateRangeWithinLimit(dates);

        var snapshots = await GetSnapshotsForDates(dates).ToListAsync(cancellationToken);

        var datesWithoutSnapshots = dates.Where(d => !snapshots.Any(s => s.Date == d)).ToList();

        var fixedTasks = datesWithoutSnapshots.Count > 0 
            ? await fixedTaskRepository.GetAll()
                .Where(ft => datesWithoutSnapshots.Contains(DateOnly.FromDateTime(ft.StartTimestamp)))
                .OrderBy(ft => ft.StartTimestamp)
                .ToListAsync(cancellationToken)
            : [];

        var dynamicTasks = datesWithoutSnapshots.Count > 0 
            ? await dynamicTaskRepository.GetAll().ToListAsync(cancellationToken)
            : [];

        var scheduledFixedTasks = datesWithoutSnapshots.Count > 0 
            ? await GetFixedTasksForScheduledTasks(dates.Min(), dates.Max(), cancellationToken).ToListAsync(cancellationToken)
            : [];

        foreach (var date in dates)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var snapshot = snapshots.FirstOrDefault(s => s.Date == date);
            if (snapshot == null)
            {
                var fixedTasksForDay = fixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                var scheduledFixedTasksForDay = scheduledFixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                var tasksForDay = taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, scheduledFixedTasksForDay, dynamicTasks, date);

                snapshot = tasksForDay.CreateOrUpdateScheduleSnapshot();
                snapshot = scheduleSnapshotRepository.Add(snapshot);
            }
            yield return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
        }

        await scheduleSnapshotRepository.SaveChangesAsync(cancellationToken);
    }

    public async IAsyncEnumerable<TasksForDayDto> RefreshTasksForDays(ICollection<DateOnly> dates, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (dates == null || dates.Count == 0)
            yield break;

        EnsureDateRangeWithinLimit(dates);

        var fixedTasks = await fixedTaskRepository.GetAll()
                                                .Where(ft => dates.Contains(DateOnly.FromDateTime(ft.StartTimestamp)))
                                                .OrderBy(ft => ft.StartTimestamp)
                                                .ToListAsync(cancellationToken);

        var dynamicTasks = await dynamicTaskRepository.GetAll().ToListAsync(cancellationToken);

        var scheduledFixedTasks = await GetFixedTasksForScheduledTasks(dates.Min(), dates.Max(), cancellationToken).ToListAsync(cancellationToken);

        // Replace existing snapshots: delete them up front in a single committed statement so the
        // regenerated rows can't collide with the old ones on the (UserId, Date) unique key.
        // (Mixing a tracked Delete + Add of the same alternate key in one SaveChanges does not
        // guarantee DELETE-before-INSERT ordering).
        await scheduleSnapshotRepository.DeleteBy(s => dates.Contains(s.Date), cancellationToken);

        foreach (var date in dates)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var fixedTasksForDay = fixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
            var scheduledFixedTasksForDay = scheduledFixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date).ToList();
            var tasksForDay = taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, scheduledFixedTasksForDay, dynamicTasks, date);

            var snapshot = tasksForDay.CreateOrUpdateScheduleSnapshot();
            scheduleSnapshotRepository.Add(snapshot);

            foreach (var scheduledFixedTasksForDayEntry in scheduledFixedTasksForDay)
                await scheduleEntityService.UpdateLastEntityCreated(scheduledFixedTasksForDayEntry.ScheduleEntityId!.Value, date, cancellationToken);

            yield return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
        }

        await scheduleSnapshotRepository.SaveChangesAsync(cancellationToken);
    }

    // Guard against a single request expanding recurrences over an unbounded span.
    private static void EnsureDateRangeWithinLimit(ICollection<DateOnly> dates)
    {
        var spanDays = (dates.Max().DayNumber - dates.Min().DayNumber) + 1;
        if (dates.Count > MaxTimelineDays || spanDays > MaxTimelineDays)
            throw new DataIsNotCorrectException($"A timeline request may span at most {MaxTimelineDays} days.", nameof(dates));
    }

    private Task<ScheduleSnapshot?> GetSnapshotForDate(DateOnly date, CancellationToken cancellationToken = default)
    {
        return scheduleSnapshotRepository.GetAll(QueryPipelineScheduleSnapshots.IncludeScheduledData)
            .FirstOrDefaultAsync(x => x.Date == date, cancellationToken);
    }

    private IAsyncEnumerable<ScheduleSnapshot> GetSnapshotsForDates(IEnumerable<DateOnly> dates)
    {
        return scheduleSnapshotRepository.GetAll(QueryPipelineScheduleSnapshots.IncludeScheduledData)
            .Where(x => dates.Contains(x.Date))
            .AsAsyncEnumerable();
    }

    private async IAsyncEnumerable<FixedTask> GetFixedTasksForScheduledTasks(DateOnly from, DateOnly? to = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var scheduleEntities = scheduleEntityService.GetAllFrom(from).AsAsyncEnumerable();

        await foreach (var scheduleEntity in scheduleEntities.WithCancellation(cancellationToken))
        {
            var taskDates = scheduleEntity.GetNextEntityDatesIn(from, to ?? from);

            foreach (var taskDate in taskDates)
            {
                var task = scheduleEntity.FixedTask!.ShallowCopy();

                var timeDifferenceInDays = task.EndTimestamp.Date - task.StartTimestamp.Date;

                task.StartTimestamp = taskDate.ToDateTime(TimeOnly.FromDateTime(task.StartTimestamp));
                task.EndTimestamp = taskDate.AddDays(timeDifferenceInDays.Days).ToDateTime(TimeOnly.FromDateTime(task.EndTimestamp));

                yield return task;
            }
        }
    }
}
