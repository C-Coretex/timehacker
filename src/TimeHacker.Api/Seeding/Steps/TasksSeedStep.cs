using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Api.Seeding.Steps;

/// <summary>
/// Seeds a handful of fixed tasks (some recurring) and dynamic tasks for the dev user. All timestamps are
/// derived from <see cref="DevelopmentSeedContext.Today"/> so the sample data always lands on the current
/// day, tomorrow, and the day after — never a stale hardcoded date.
/// </summary>
internal sealed class TasksSeedStep : IDevelopmentSeedStep
{
    public async Task SeedAsync(DevelopmentSeedContext context, CancellationToken cancellationToken)
    {
        await SeedFixedTasksAsync(context, cancellationToken);
        await SeedDynamicTasksAsync(context, cancellationToken);
    }

    private static async Task SeedFixedTasksAsync(DevelopmentSeedContext context, CancellationToken cancellationToken)
    {
        var fixedTasks = context.Db.Set<FixedTask>();
        var alreadySeeded = await fixedTasks.AnyAsync(x => x.UserId == context.UserId, cancellationToken);
        if (alreadySeeded)
            return;

        var today = context.Today;
        var tomorrow = today.AddDays(1);
        var dayAfter = today.AddDays(2);

        // Recurring blueprints are inserted first so their generated ids can be attached to the tasks below.
        // Each is anchored to its task's own day, so the recurrence resumes after it instead of
        // regenerating that day on top of the task itself.
        var dailyStandup = context.NewSchedule(
            new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(1)),
            today
        );
        var weeklySync = context.NewSchedule(
            new RepeatingEntityDto(RepeatingEntityType.WeekRepeatingEntity, new WeekRepeatingEntity([dayAfter.DayOfWeek.ToDayOfWeek()])),
            dayAfter
        );

        context.Db.Set<ScheduleEntity>().AddRange(dailyStandup, weeklySync);
        await context.Db.SaveChangesAsync(cancellationToken);

        context.Db.Set<FixedTask>().AddRange(
            NewFixedTask(context, "Morning standup", priority: 4, today, 9, 0, 9, 30, dailyStandup.Id),
            NewFixedTask(context, "Lunch break", priority: 2, today, 12, 30, 13, 0),
            NewFixedTask(context, "Gym session", priority: 6, today, 18, 0, 19, 0),
            NewFixedTask(context, "Dentist appointment", priority: 8, tomorrow, 10, 0, 11, 0),
            NewFixedTask(context, "Team sync", priority: 5, dayAfter, 14, 0, 15, 0, weeklySync.Id));

        await context.Db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDynamicTasksAsync(DevelopmentSeedContext context, CancellationToken cancellationToken)
    {
        var dynamicTasks = context.Db.Set<DynamicTask>();
        var alreadySeeded = await dynamicTasks.AnyAsync(x => x.UserId == context.UserId, cancellationToken);
        if (alreadySeeded)
            return;

        context.Db.Set<DynamicTask>().AddRange(
            NewDynamicTask(context, "Read a book", priority: 3, min: 20, max: 60, optimal: 30),
            NewDynamicTask(context, "Learn Spanish", priority: 5, min: 15, max: 45),
            NewDynamicTask(context, "Code review", priority: 7, min: 30, max: 90, optimal: 45),
            NewDynamicTask(context, "Meditate", priority: 4, min: 10, max: 20));

        await context.Db.SaveChangesAsync(cancellationToken);
    }

    private static FixedTask NewFixedTask(
        DevelopmentSeedContext context,
        string name,
        byte priority,
        DateOnly date,
        int startHour,
        int startMinute,
        int endHour,
        int endMinute,
        Guid? scheduleEntityId = null)
        => new()
        {
            UserId = context.UserId,
            CreatedTimestamp = context.Now,
            Name = name,
            Priority = priority,
            StartTimestamp = date.ToDateTime(new TimeOnly(startHour, startMinute), DateTimeKind.Utc),
            EndTimestamp = date.ToDateTime(new TimeOnly(endHour, endMinute), DateTimeKind.Utc),
            ScheduleEntityId = scheduleEntityId
        };

    private static DynamicTask NewDynamicTask(
        DevelopmentSeedContext context,
        string name,
        byte priority,
        int min,
        int max,
        int? optimal = null)
        => new()
        {
            UserId = context.UserId,
            CreatedTimestamp = context.Now,
            Name = name,
            Priority = priority,
            MinTimeToFinish = TimeSpan.FromMinutes(min),
            MaxTimeToFinish = TimeSpan.FromMinutes(max),
            OptimalTimeToFinish = optimal is null ? null : TimeSpan.FromMinutes(optimal.Value)
        };
}
