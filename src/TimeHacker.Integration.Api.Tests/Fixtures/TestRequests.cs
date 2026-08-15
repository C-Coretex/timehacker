using System.Drawing;
using System.Globalization;
using TimeHacker.Api.Models.Input.Categories;
using TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;
using TimeHacker.Api.Models.Input.Users;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
using DomainDayOfWeek = TimeHacker.Domain.Models.EntityModels.Enums.DayOfWeek;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

// Central factories for API request bodies so the test files stay short and consistent.
internal static class TestRequests
{
    public static InputCategoryModel NewCategory(string name = "Work", Color? color = null, string? description = "work stuff")
        => new() { Name = name, Description = description, Color = color ?? Color.Blue };

    public static InputFixedTaskModel NewFixedTask(
        string name = "Standup",
        DateTime? start = null,
        DateTime? end = null,
        byte priority = 5,
        IEnumerable<Guid>? categoryIds = null,
        string? description = "daily standup")
    {
        var startValue = start ?? new DateTime(2026, 07, 01, 09, 00, 00, DateTimeKind.Utc);
        var endValue = end ?? startValue.AddHours(1);

        return new InputFixedTaskModel
        {
            Name = name,
            Description = description,
            Priority = priority,
            CategoryIds = categoryIds ?? [],
            StartTimestamp = Iso(startValue),
            EndTimestamp = Iso(endValue)
        };
    }

    public static InputDynamicTaskModel NewDynamicTask(
        string name = "Read book",
        TimeSpan? min = null,
        TimeSpan? max = null,
        TimeSpan? optimal = null,
        byte priority = 5,
        string? description = "reading")
        => new()
        {
            Name = name,
            Description = description,
            Priority = priority,
            MinTimeToFinish = min ?? TimeSpan.FromMinutes(30),
            MaxTimeToFinish = max ?? TimeSpan.FromMinutes(60),
            OptimalTimeToFinish = optimal
        };

    public static UserUpdateModel NewUserUpdate(
        string name = "Updated Name",
        string? phone = "+15551234567",
        string? email = "notify@test.local",
        DateOnly? birthday = null)
        => new()
        {
            Name = name,
            PhoneNumberForNotifications = phone,
            EmailForNotifications = email,
            Birthday = birthday
        };

    public static InputScheduleEntityModel NewSchedule(
        Guid parentFixedTaskId,
        InputRepeatingEntityModelBase repeating,
        EndsOnModel? endsOn = null)
        => new() { ParentEntityId = parentFixedTaskId, RepeatingEntityType = repeating, EndsOnModel = endsOn };

    // --- Repeating-entity builders ---
    public static InputDayRepeatingEntityModel EveryNDays(int days = 1) => new() { DaysCountToRepeat = days };

    public static InputWeekRepeatingEntityModel EveryWeekOn(params DomainDayOfWeek[] days) => new() { RepeatsOn = days };

    public static InputMonthRepeatingEntityModel EveryMonthOnDay(byte day) => new() { MonthDayToRepeat = day };

    public static InputYearRepeatingEntityModel EveryYearOnDay(int day) => new() { YearDayToRepeat = day };

    // Naive ISO string; the API's model parses it as UTC (AssumeUniversal).
    private static string Iso(DateTime value) => value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
}
