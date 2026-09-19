using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Api.Seeding.Steps;

/// <summary>
/// Seeds a handful of fixed tasks (some recurring) and dynamic tasks for the dev user. All timestamps are
/// derived from <see cref="DevelopmentSeedContext.Today"/> so the sample data always lands on the current
/// day, tomorrow, and the day after — never a stale hardcoded date.
///
/// Runs after <see cref="CategoriesSeedStep"/> and links the seeded tasks to those categories by name.
/// Every dynamic task gets at least one, as the domain requires; the fixed tasks deliberately include an
/// uncategorised one, since for them a category is optional.
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

        var standup = NewFixedTask(context, "Morning standup", priority: 4, today, 9, 0, 9, 30, dailyStandup.Id);
        var lunch = NewFixedTask(context, "Lunch break", priority: 2, today, 12, 30, 13, 0);
        var gymSession = NewFixedTask(context, "Gym session", priority: 6, today, 18, 0, 19, 0);
        var dentist = NewFixedTask(context, "Dentist appointment", priority: 8, tomorrow, 10, 0, 11, 0);
        var teamSync = NewFixedTask(context, "Team sync", priority: 5, dayAfter, 14, 0, 15, 0, weeklySync.Id);

        context.Db.Set<FixedTask>().AddRange(standup, lunch, gymSession, dentist, teamSync);
        await context.Db.SaveChangesAsync(cancellationToken);

        var categories = await LoadCategoriesByNameAsync(context, cancellationToken);

        context.Db.Set<CategoryFixedTask>().AddRange([
            .. Link(standup, categories, "Work"),
            // Two categories on one task, to show the junction is many-to-many in both directions.
            .. Link(gymSession, categories, "Gym", "Family time"),
            .. Link(dentist, categories, "Family time"),
            .. Link(teamSync, categories, "Work", "Deep work")]);
        // "Lunch break" is left uncategorised on purpose: a fixed task does not need one.

        await context.Db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDynamicTasksAsync(DevelopmentSeedContext context, CancellationToken cancellationToken)
    {
        var dynamicTasks = context.Db.Set<DynamicTask>();
        var alreadySeeded = await dynamicTasks.AnyAsync(x => x.UserId == context.UserId, cancellationToken);
        if (alreadySeeded)
            return;

        var readBook = NewDynamicTask(context, "Read a book", priority: 3, min: 20, max: 60, optimal: 30);
        var spanish = NewDynamicTask(context, "Learn Spanish", priority: 5, min: 15, max: 45);
        var codeReview = NewDynamicTask(context, "Code review", priority: 7, min: 30, max: 90, optimal: 45);
        var meditate = NewDynamicTask(context, "Meditate", priority: 4, min: 10, max: 20);

        context.Db.Set<DynamicTask>().AddRange(readBook, spanish, codeReview, meditate);
        await context.Db.SaveChangesAsync(cancellationToken);

        var categories = await LoadCategoriesByNameAsync(context, cancellationToken);

        // Every dynamic task is linked: the domain rejects one without a category.
        context.Db.Set<CategoryDynamicTask>().AddRange([
            .. Link(readBook, categories, "Family time"),
            .. Link(spanish, categories, "Deep work"),
            .. Link(codeReview, categories, "Work", "Deep work"),
            .. Link(meditate, categories, "Gym")]);

        await context.Db.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Dictionary<string, Guid>> LoadCategoriesByNameAsync(
        DevelopmentSeedContext context,
        CancellationToken cancellationToken)
        => await context.Db.Set<Category>()
            .Where(category => category.UserId == context.UserId)
            .ToDictionaryAsync(category => category.Name, category => category.Id, cancellationToken);

    // Names come from CategoriesSeedStep. An unknown one is skipped rather than throwing: this runs on
    // every dev startup, and a database seeded by an older version may not carry that category.
    private static IEnumerable<CategoryFixedTask> Link(FixedTask task, IReadOnlyDictionary<string, Guid> categories, params IEnumerable<string> names)
        => names.Where(categories.ContainsKey)
                .Select(name => new CategoryFixedTask { CategoryId = categories[name], FixedTaskId = task.Id });

    private static IEnumerable<CategoryDynamicTask> Link(DynamicTask task, IReadOnlyDictionary<string, Guid> categories, params IEnumerable<string> names)
        => names.Where(categories.ContainsKey)
                .Select(name => new CategoryDynamicTask { CategoryId = categories[name], DynamicTaskId = task.Id });

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
