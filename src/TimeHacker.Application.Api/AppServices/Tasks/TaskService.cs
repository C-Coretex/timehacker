using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using TimeHacker.Application.Api.QueryPipelineSteps;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.Observability;

namespace TimeHacker.Application.Api.AppServices.Tasks;

public class TaskService(
    IFixedTaskRepository fixedTaskRepository,
    IDynamicTaskRepository dynamicTaskRepository,
    IScheduleSnapshotRepository scheduleSnapshotRepository,
    IScheduleEntityService scheduleEntityService,
    ITaskTimelineProcessor taskTimelineProcessor,
    UserAccessorBase userAccessor) : ITaskAppService
{
    // Upper bound on how many days a single timeline request may span / generate.
    private const int MaxTimelineDays = 366;

    /// <returns>
    /// The day's timeline, snapshot-first. If a snapshot already exists it is returned as-is;
    /// otherwise the three task sources (one-off fixed, recurrence-generated fixed, and dynamic) are run
    /// through the timeline processor and the result is persisted as a snapshot. The snapshot freezes the
    /// (randomized) generation so the same day always reads back the same plan.
    /// </returns>
    public async Task<TasksForDayDto> GetTasksForDay(DateOnly date, CancellationToken cancellationToken = default)
    {
        var snapshot = await GetSnapshotForDate(date, cancellationToken);
        if (snapshot != null)
        {
            RecordSnapshotHit();
            return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
        }

        var fixedTasks = await fixedTaskRepository.GetAll()
                                          .Where(ft => DateOnly.FromDateTime(ft.StartTimestamp) == date)
                                          .OrderBy(ft => ft.StartTimestamp)
                                          .ToListAsync(cancellationToken);

        var dynamicTasks = await dynamicTaskRepository.GetAll().ToListAsync(cancellationToken);

        var scheduledFixedTasks = await GetFixedTasksForScheduledTasks(date, cancellationToken: cancellationToken).ToListAsync(cancellationToken);

        //for now we discard dates, but when we use categories for scheduling we would use the dates too
        var categories = await GetCategoriesForScheduledCategories(date, cancellationToken: cancellationToken)
            .Select(x => x.Category)
            .ToListAsync(cancellationToken);

        var tasksForDay = GenerateTimeline(fixedTasks, scheduledFixedTasks, dynamicTasks, categories, date);

        snapshot = tasksForDay.CreateOrUpdateScheduleSnapshot();
        snapshot = await scheduleSnapshotRepository.AddAndSaveAsync(snapshot, cancellationToken);

        return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
    }

    /// <returns>
    /// Stream of timelines for many dates. Existing snapshots are reused; only the missing dates trigger task
    /// loading. Newly generated snapshots are tracked-Added during the loop and committed in a single SaveChanges 
    /// after the last yield.
    /// </returns>
    public async IAsyncEnumerable<TasksForDayDto> GetTasksForDays(ICollection<DateOnly> dates, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (dates == null || dates.Count == 0)
            yield break;

        EnsureDateRangeWithinLimit(dates);

        var snapshots = await GetSnapshotsForDates(dates).ToListAsync(cancellationToken);

        // Only the dates without a snapshot need fresh generation; if there are none, skip all task queries.
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

        var scheduledCategories = datesWithoutSnapshots.Count > 0
            ? await GetCategoriesForScheduledCategories(dates.Min(), dates.Max(), cancellationToken).ToListAsync(cancellationToken)
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
                var categoriesForDay = scheduledCategories.Where(c => c.Date == date).Select(c => c.Category);
                var tasksForDay = GenerateTimeline(fixedTasksForDay, scheduledFixedTasksForDay, dynamicTasks, categoriesForDay, date);

                snapshot = tasksForDay.CreateOrUpdateScheduleSnapshot();
                // Add to the change tracker but defer the commit until after all dates are yielded.
                snapshot = scheduleSnapshotRepository.Add(snapshot);
            }
            else
                RecordSnapshotHit();

            yield return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
        }

        await scheduleSnapshotRepository.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Discards and regenerates the snapshots for the given dates (used when the underlying tasks/schedules
    /// changed). Unlike <see cref="GetTasksForDays"/>, existing snapshots are not reused.
    /// </summary>
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

        var scheduledCategories = await GetCategoriesForScheduledCategories(dates.Min(), dates.Max(), cancellationToken).ToListAsync(cancellationToken);

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
            var scheduledCategoriesForDay = scheduledCategories.Where(c => c.Date == date).Select(c => c.Category).ToList();
            var tasksForDay = GenerateTimeline(fixedTasksForDay, scheduledFixedTasksForDay, dynamicTasks, scheduledCategoriesForDay, date);

            var snapshot = tasksForDay.CreateOrUpdateScheduleSnapshot();
            scheduleSnapshotRepository.Add(snapshot);
            TimeHackerTelemetry.SnapshotsRefreshed.Add(1);

            // Advance each recurrence's progress marker to this date so future generation resumes correctly.
            foreach (var scheduledFixedTasksForDayEntry in scheduledFixedTasksForDay)
                await scheduleEntityService.UpdateLastEntityCreated(scheduledFixedTasksForDayEntry.ScheduleEntityId!.Value, date, cancellationToken);

            foreach (var scheduledCategoryForDay in scheduledCategoriesForDay)
                await scheduleEntityService.UpdateLastEntityCreated(scheduledCategoryForDay.ScheduleEntityId!.Value, date, cancellationToken);

            yield return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
        }

        await scheduleSnapshotRepository.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Runs the timeline-generation algorithm for a single day inside a business span and records the
    /// generation metrics (duration, generated-task count, and a <c>generated</c> snapshot-request tally).
    /// The snapshot-hit path calls <see cref="RecordSnapshotHit"/> instead.
    /// </summary>
    private TasksForDayReturn GenerateTimeline(
        IEnumerable<FixedTask> fixedTasks,
        IEnumerable<FixedTask> scheduledFixedTasks,
        IEnumerable<DynamicTask> dynamicTasks,
        IEnumerable<Category> categories,
        DateOnly date)
    {
        using var activity = TimeHackerTelemetry.ActivitySource.StartActivity("timeline.generate");

        activity?.SetTag("timehacker.date", date.ToString("O", CultureInfo.InvariantCulture));
        if (userAccessor.UserId is { } userId)
            activity?.SetTag("enduser.id", userId.ToString());

        var startTimestamp = Stopwatch.GetTimestamp();
        var tasksForDay = taskTimelineProcessor.GetTasksForDay(fixedTasks, scheduledFixedTasks, dynamicTasks, categories, date);
        var elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

        var taskCount = tasksForDay.TasksTimeline.Count;
        activity?.SetTag("timehacker.tasks.count", taskCount);
        activity?.SetTag("timehacker.categories.count", tasksForDay.CategoriesTimeline.Count);

        TimeHackerTelemetry.SnapshotRequests.Add(1, new KeyValuePair<string, object?>("outcome", TimeHackerTelemetry.OutcomeGenerated));
        TimeHackerTelemetry.TimelineGenerationDuration.Record(elapsedMs);
        TimeHackerTelemetry.ScheduledTasksGenerated.Add(taskCount);

        return tasksForDay;
    }

    //TODO: probably we don't need this metric, as Snapshot is not cache. We would be more interested in how many snapshots are generated.
    private static void RecordSnapshotHit() =>
        TimeHackerTelemetry.SnapshotRequests.Add(1, new KeyValuePair<string, object?>("outcome", TimeHackerTelemetry.OutcomeSnapshotHit));

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

    /// <summary>
    /// Materializes active category recurrences into the concrete <see cref="Category"/> instances that apply
    /// on each occurrence date in the range, paired with that date. A category carries a time-of-day window
    /// rather than a timestamp, so unlike the fixed-task expansion nothing needs shifting — only the set of
    /// days it lands on is computed here.
    /// </summary>
    private async IAsyncEnumerable<(DateOnly Date, Category Category)> GetCategoriesForScheduledCategories(DateOnly from, DateOnly? to = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var scheduleEntities = scheduleEntityService.GetAllCategoriesFrom(from).AsAsyncEnumerable();

        await foreach (var scheduleEntity in scheduleEntities.WithCancellation(cancellationToken))
        {
            var categoryDates = scheduleEntity.GetNextEntityDatesIn(from, to ?? from).ToList();

            TimeHackerTelemetry.ScheduleEntitiesExpanded.Add(categoryDates.Count,
                new KeyValuePair<string, object?>("repeating_type", scheduleEntity.RepeatingEntity.EntityType.ToString()));

            foreach (var categoryDate in categoryDates)
                yield return (categoryDate, scheduleEntity.Category!);
        }
    }

    /// <summary>
    /// Materializes active recurrences into concrete FixedTask instances for each occurrence date in the range.
    /// Each occurrence is a shallow copy of the template task with its timestamps shifted onto the target date,
    /// preserving both the original time-of-day and the original multi-day span (end may land on a later day).
    /// </summary>
    private async IAsyncEnumerable<FixedTask> GetFixedTasksForScheduledTasks(DateOnly from, DateOnly? to = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var scheduleEntities = scheduleEntityService.GetAllFrom(from).AsAsyncEnumerable();

        await foreach (var scheduleEntity in scheduleEntities.WithCancellation(cancellationToken))
        {
            var taskDates = scheduleEntity.GetNextEntityDatesIn(from, to ?? from).ToList();

            // Which recurrence patterns actually drive generation load — a daily blueprint over a year-long
            // request expands into far more work than a yearly one.
            TimeHackerTelemetry.ScheduleEntitiesExpanded.Add(taskDates.Count,
                new KeyValuePair<string, object?>("repeating_type", scheduleEntity.RepeatingEntity.EntityType.ToString()));

            foreach (var taskDate in taskDates)
            {
                var task = scheduleEntity.FixedTask!.ShallowCopy();

                // Preserve how many days the task originally spanned so the shifted end keeps that duration.
                var timeDifferenceInDays = task.EndTimestamp.Date - task.StartTimestamp.Date;

                task.StartTimestamp = taskDate.ToDateTime(TimeOnly.FromDateTime(task.StartTimestamp));
                task.EndTimestamp = taskDate.AddDays(timeDifferenceInDays.Days).ToDateTime(TimeOnly.FromDateTime(task.EndTimestamp));

                yield return task;
            }
        }
    }
}
