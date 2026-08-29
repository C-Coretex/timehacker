using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

using DomainDayOfWeek = TimeHacker.Domain.Models.EntityModels.Enums.DayOfWeek;

namespace TimeHacker.Api.Seeding.Steps;

/// <summary>
/// All dates derive from <see cref="DevelopmentSeedContext.Today"/>, never a hardcoded date.
/// </summary>
internal sealed class CategoriesSeedStep : IDevelopmentSeedStep
{
    public async Task SeedAsync(DevelopmentSeedContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        var categories = context.Db.Set<Category>();
        var alreadySeeded = await categories.AnyAsync(x => x.UserId == context.UserId, cancellationToken);
        if (alreadySeeded)
            return;

        var today = context.Today;
        var tomorrow = today.AddDays(1);
        var dayAfter = today.AddDays(2);

        // A category always lands on its own date, recurrence or not — so anchoring the weekday-only one
        // on a weekend would show "Work" on a Saturday purely because that is when the seed ran.
        var nextWeekday = Enumerable.Range(0, 3)
            .Select(today.AddDays)
            .First(date => date.DayOfWeek is not (System.DayOfWeek.Saturday or System.DayOfWeek.Sunday));

        // Blueprints go in first so their generated ids can be attached to the categories below. Each is
        // anchored to its category's own day, so the recurrence resumes after it rather than regenerating
        // that day on top of the category itself.
        var everyWeekday = context.NewSchedule(
            new RepeatingEntityDto(
                RepeatingEntityType.WeekRepeatingEntity,
                new WeekRepeatingEntity([
                    DomainDayOfWeek.Monday, DomainDayOfWeek.Tuesday, DomainDayOfWeek.Wednesday,
                    DomainDayOfWeek.Thursday, DomainDayOfWeek.Friday
                ])),
            nextWeekday);

        var everyOtherDay = context.NewSchedule(
            new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2)),
            today);

        // A finite list: every date must be strictly after the anchor, or the schedule could never fire.
        var onChosenDays = context.NewSchedule(
            new RepeatingEntityDto(
                RepeatingEntityType.OnceRepeatingEntity,
                new OnceRepeatingEntity([tomorrow.AddDays(1), tomorrow.AddDays(4), tomorrow.AddDays(8)])),
            tomorrow);

        var everyMonth = context.NewSchedule(
            new RepeatingEntityDto(RepeatingEntityType.MonthRepeatingEntity, new MonthRepeatingEntity((byte)dayAfter.Day)),
            dayAfter);

        var everyYear = context.NewSchedule(
            new RepeatingEntityDto(RepeatingEntityType.YearRepeatingEntity, new YearRepeatingEntity(dayAfter.DayOfYear)),
            dayAfter);

        context.Db.Set<ScheduleEntity>().AddRange(everyWeekday, everyOtherDay, onChosenDays, everyMonth, everyYear);
        await context.Db.SaveChangesAsync(cancellationToken);

        context.Db.Set<Category>().AddRange(
            NewCategory(context, "Work", Color.SteelBlue, nextWeekday, 9, 0, 18, 0, everyWeekday.Id,
                "Working hours"),
            NewCategory(context, "Gym", Color.IndianRed, today, 18, 30, 20, 0, everyOtherDay.Id,
                "Every other day"),
            // No schedule at all: it still lands on its own date, exactly as a fixed task does.
            NewCategory(context, "Family time", Color.MediumSeaGreen, tomorrow, 19, 0, 21, 0,
                description: "One-off, no recurrence"),
            NewCategory(context, "Deep work", Color.DarkOrange, tomorrow, 10, 0, 12, 0, onChosenDays.Id,
                "On a few chosen days"),
            NewCategory(context, "Monthly review", Color.MediumPurple, dayAfter, 15, 0, 16, 0, everyMonth.Id,
                "Same day each month"),
            NewCategory(context, "Annual planning", Color.Teal, dayAfter, 8, 0, 9, 0, everyYear.Id,
                "Same day each year"));

        await context.Db.SaveChangesAsync(cancellationToken);
    }

    private static Category NewCategory(
        DevelopmentSeedContext context,
        string name,
        Color color,
        DateOnly date,
        int startHour,
        int startMinute,
        int endHour,
        int endMinute,
        Guid? scheduleEntityId = null,
        string? description = null)
        => new()
        {
            UserId = context.UserId,
            CreatedTimestamp = context.Now,
            Name = name,
            Description = description,
            Color = color,
            Date = date,
            // Wall-clock, never UTC-converted — so an overnight window would not work here.
            StartTime = new TimeOnly(startHour, startMinute),
            EndTime = new TimeOnly(endHour, endMinute),
            ScheduleEntityId = scheduleEntityId
        };
}
