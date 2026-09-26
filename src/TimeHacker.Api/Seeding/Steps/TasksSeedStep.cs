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

        // Priorities use the 1 (Highest) – 5 (Lowest) scale from PriorityConstants.
        // Every task carries a description, since the planner shows it on the event whenever there is room.
        var standup = NewFixedTask(context, "Morning standup", "Yesterday, today and blockers, in five minutes each.", priority: 2, today, 9, 0, 9, 30, dailyStandup.Id);
        var lunch = NewFixedTask(context, "Lunch break", "Away from the screen, a proper meal.", priority: 5, today, 12, 30, 13, 0);
        var gymSession = NewFixedTask(context, "Gym session", "Legs and core, then ten minutes of stretching.", priority: 3, today, 18, 0, 19, 0);
        var dentist = NewFixedTask(context, "Dentist appointment", "Regular check-up. Bring the insurance card.", priority: 1, tomorrow, 10, 0, 11, 0);
        var teamSync = NewFixedTask(context, "Team sync", "Weekly planning with the whole team.", priority: 3, dayAfter, 14, 0, 15, 0, weeklySync.Id);

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

        var readBook = NewDynamicTask(context, "Read a book", "A chapter of the current novel, phone in another room.", priority: 3, min: 20, max: 60, optimal: 30);
        var spanish = NewDynamicTask(context, "Learn Spanish", "One lesson plus ten flashcards.", priority: 2, min: 15, max: 45);
        var codeReview = NewDynamicTask(context, "Code review", "Open pull requests, oldest first.", priority: 1, min: 30, max: 90, optimal: 45);
        var meditate = NewDynamicTask(context, "Meditate", "Breathing exercise with the timer on.", priority: 4, min: 10, max: 20);

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
        string description,
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
            Description = description,
            Priority = priority,
            StartTimestamp = date.ToDateTime(new TimeOnly(startHour, startMinute), DateTimeKind.Utc),
            EndTimestamp = date.ToDateTime(new TimeOnly(endHour, endMinute), DateTimeKind.Utc),
            ScheduleEntityId = scheduleEntityId
        };

    private static DynamicTask NewDynamicTask(
        DevelopmentSeedContext context,
        string name,
        string description,
        byte priority,
        int min,
        int max,
        int? optimal = null)
        => new()
        {
            UserId = context.UserId,
            CreatedTimestamp = context.Now,
            Name = name,
            Description = description,
            Priority = priority,
            MinTimeToFinish = TimeSpan.FromMinutes(min),
            MaxTimeToFinish = TimeSpan.FromMinutes(max),
            OptimalTimeToFinish = optimal is null ? null : TimeSpan.FromMinutes(optimal.Value)
        };
}
