using System.Text.Json;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Integration.Api.Tests.Categories;

/// <summary>
/// The nested /api/categories/{categoryId}/schedules endpoints, plus the recurrence attach
/// (/api/categories/schedules) whose ParentEntityId is a category-schedule id.
/// </summary>
public sealed class CategorySchedulesApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    private async Task<(TimeHackerApi Api, Guid CategoryId)> CreateCategoryAsync(string name = "Work")
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = (await api.Categories.Create(TestRequests.NewCategory(name))).Content;
        return (api, categoryId);
    }

    [Fact, Trait("Endpoint", "POST+GET /api/categories/{categoryId}/schedules")]
    public async Task Create_Should_PersistAndRoundTrip()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var date = Today.AddDays(3);

        var create = await api.Categories.CreateSchedule(categoryId,
            TestRequests.NewCategorySchedule("Working hours", date, new TimeOnly(09, 30), new TimeOnly(17, 45)));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var schedules = await api.Categories.GetSchedules(categoryId);
        schedules.StatusCode.Should().Be(HttpStatusCode.OK);

        var stored = schedules.Content!.Should().ContainSingle().Subject;
        stored.Id.Should().Be(create.Content);
        stored.CategoryId.Should().Be(categoryId);
        stored.Description.Should().Be("Working hours");
        stored.Date.Should().Be(date);
        // Wall-clock values, stored exactly as entered and never UTC-converted.
        stored.StartTime.Should().Be(new TimeOnly(09, 30));
        stored.EndTime.Should().Be(new TimeOnly(17, 45));
        stored.ScheduleEntity.Should().BeNull();

        (await AdminDbContext.Set<CategorySchedule>().CountAsync(cancellationToken)).Should().Be(1);
    }

    [Fact, Trait("Endpoint", "POST /api/categories/{categoryId}/schedules")]
    public async Task Create_Should_AllowSeveralWindowsOnTheSameDay()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var date = Today.AddDays(1);

        await api.Categories.CreateSchedule(categoryId,
            TestRequests.NewCategorySchedule("Working hours", date, new TimeOnly(09, 00), new TimeOnly(12, 00)));
        await api.Categories.CreateSchedule(categoryId,
            TestRequests.NewCategorySchedule("Overtime", date, new TimeOnly(18, 00), new TimeOnly(20, 00)));
        // Overlapping the first window is allowed too — nothing de-duplicates or resolves conflicts.
        await api.Categories.CreateSchedule(categoryId,
            TestRequests.NewCategorySchedule("Meetings", date, new TimeOnly(10, 00), new TimeOnly(11, 00)));

        var schedules = await api.Categories.GetSchedules(categoryId);

        schedules.Content!.Should().HaveCount(3);
        schedules.Content!.Select(s => s.Description).Should().BeEquivalentTo("Working hours", "Overtime", "Meetings");
        schedules.Content!.Select(s => s.Date).Should().AllBeEquivalentTo(date);
    }

    [Fact, Trait("Endpoint", "GET /api/categories/{categoryId}/schedules")]
    public async Task GetSchedules_Should_ReturnOnlyThatCategorysWindows()
    {
        var (api, workId) = await CreateCategoryAsync("Work");
        var gymId = (await api.Categories.Create(TestRequests.NewCategory("Gym"))).Content;

        await api.Categories.CreateSchedule(workId, TestRequests.NewCategorySchedule("Working hours"));
        await api.Categories.CreateSchedule(gymId, TestRequests.NewCategorySchedule("Workout"));

        var schedules = await api.Categories.GetSchedules(workId);

        schedules.Content!.Should().ContainSingle().Which.Description.Should().Be("Working hours");
    }

    [Fact, Trait("Endpoint", "PUT /api/categories/{categoryId}/schedules/{id}")]
    public async Task Update_Should_ChangeTheWindow()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var scheduleId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("Working hours"))).Content;
        var newDate = Today.AddDays(9);

        var update = await api.Categories.UpdateSchedule(categoryId, scheduleId,
            TestRequests.NewCategorySchedule("Renamed", newDate, new TimeOnly(07, 30), new TimeOnly(12, 45)));
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var stored = (await api.Categories.GetSchedules(categoryId)).Content!.Single();
        stored.Description.Should().Be("Renamed");
        stored.Date.Should().Be(newDate);
        stored.StartTime.Should().Be(new TimeOnly(07, 30));
        stored.EndTime.Should().Be(new TimeOnly(12, 45));
    }

    [Fact, Trait("Endpoint", "PUT /api/categories/{categoryId}/schedules/{id}")]
    public async Task Update_Should_KeepAttachedRecurrence()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var scheduleId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("Working hours"))).Content;
        var recurrenceId = (await api.Categories.CreateRecurrence(
            TestRequests.NewSchedule(scheduleId, TestRequests.EveryNDays(1)))).Content!.Id;

        // The window payload carries no recurrence link, so writing it back must not clear it.
        await api.Categories.UpdateSchedule(categoryId, scheduleId, TestRequests.NewCategorySchedule("Renamed"));

        var stored = (await api.Categories.GetSchedules(categoryId)).Content!.Single();
        stored.Description.Should().Be("Renamed");
        stored.ScheduleEntity!.Id.Should().Be(recurrenceId);
    }

    [Fact, Trait("Endpoint", "DELETE /api/categories/{categoryId}/schedules/{id}")]
    public async Task Delete_Should_Return204_AndRemoveRow()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var scheduleId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("Working hours"))).Content;

        var delete = await api.Categories.DeleteSchedule(categoryId, scheduleId);
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await api.Categories.GetSchedules(categoryId)).Content!.Should().BeEmpty();
        (await AdminDbContext.Set<CategorySchedule>().CountAsync(cancellationToken)).Should().Be(0);
        // The category itself survives — only the window was removed.
        (await AdminDbContext.Set<Category>().CountAsync(cancellationToken)).Should().Be(1);
    }

    [Fact, Trait("Cascade", "CategorySchedule->ScheduleEntity")]
    public async Task Delete_Should_CascadeItsRecurrence()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        var keptId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("Kept"))).Content;
        await api.Categories.CreateRecurrence(TestRequests.NewSchedule(keptId, TestRequests.EveryNDays(1)));

        var doomedId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("Doomed"))).Content;
        await api.Categories.CreateRecurrence(TestRequests.NewSchedule(doomedId, TestRequests.EveryNDays(1)));

        (await AdminDbContext.Set<ScheduleEntity>().CountAsync(cancellationToken)).Should().Be(2);

        (await api.Categories.DeleteSchedule(categoryId, doomedId)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Only the deleted window's recurrence goes; the sibling's is untouched.
        (await AdminDbContext.Set<ScheduleEntity>().CountAsync(cancellationToken)).Should().Be(1);
        (await AdminDbContext.Set<CategorySchedule>().CountAsync(cancellationToken)).Should().Be(1);
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Create_Should_Return404_ForUnknownCategory()
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Categories.CreateSchedule(Guid.CreateVersion7(), TestRequests.NewCategorySchedule());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.GetProperty("title").GetString().Should().Be("Resource is not found.");
        body.RootElement.GetProperty("ResourceName").GetString().Should().Be("Category");
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Update_Delete_Should_Return404_ForUnknownId()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var unknown = Guid.CreateVersion7();

        (await api.Categories.UpdateSchedule(categoryId, unknown, TestRequests.NewCategorySchedule()))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);

        var delete = await api.Categories.DeleteSchedule(categoryId, unknown);
        delete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var body = JsonDocument.Parse(((ApiException)delete.Error!).Content!);
        body.RootElement.GetProperty("title").GetString().Should().Be("Resource is not found.");
        body.RootElement.GetProperty("ResourceName").GetString().Should().Be("CategorySchedule");
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Delete_Should_Return404_WhenCategoryDoesNotOwnTheWindow()
    {
        var (api, workId) = await CreateCategoryAsync("Work");
        var gymId = (await api.Categories.Create(TestRequests.NewCategory("Gym"))).Content;
        var scheduleId = (await api.Categories.CreateSchedule(workId, TestRequests.NewCategorySchedule("Working hours"))).Content;

        // The route's categoryId is part of the delete predicate, so another category cannot reach it.
        (await api.Categories.DeleteSchedule(gymId, scheduleId)).StatusCode.Should().Be(HttpStatusCode.NotFound);

        (await api.Categories.GetSchedules(workId)).Content!.Should().ContainSingle();
    }

    [Fact, Trait("Endpoint", "Validation")]
    public async Task Create_Should_Return400_WhenEndTimeNotAfterStartTime()
    {
        var (api, categoryId) = await CreateCategoryAsync();

        var response = await api.Categories.CreateSchedule(categoryId,
            TestRequests.NewCategorySchedule(startTime: new TimeOnly(18, 00), endTime: new TimeOnly(09, 00)));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.GetProperty("title").GetString().Should().Be("Parameter is not correct.");
    }

    [Fact, Trait("Endpoint", "POST /api/categories/{categoryId}/schedules")]
    public async Task Create_Should_Succeed_WithoutADescription()
    {
        var (api, categoryId) = await CreateCategoryAsync();

        // The description is optional — the category's name is what identifies the window.
        var response = await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule(description: null));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await api.Categories.GetSchedules(categoryId)).Content!
            .Should().ContainSingle().Which.Description.Should().BeNull();
    }

    [Fact, Trait("Endpoint", "PUT /api/categories/{categoryId}/schedules/{id}")]
    public async Task Update_Should_ClearTheDescription()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var scheduleId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("Working hours"))).Content;

        var update = await api.Categories.UpdateSchedule(categoryId, scheduleId, TestRequests.NewCategorySchedule(description: null));

        update.StatusCode.Should().Be(HttpStatusCode.OK);
        (await api.Categories.GetSchedules(categoryId)).Content!.Single().Description.Should().BeNull();
    }

    [Fact, Trait("Endpoint", "Validation")]
    public async Task Update_Should_Return400_WhenEndTimeNotAfterStartTime()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var scheduleId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule())).Content;

        var response = await api.Categories.UpdateSchedule(categoryId, scheduleId,
            TestRequests.NewCategorySchedule(startTime: new TimeOnly(18, 00), endTime: new TimeOnly(09, 00)));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // --- Recurrence attach ---

    [Fact, Trait("Endpoint", "POST /api/categories/schedules")]
    public async Task CreateRecurrence_Should_LinkToTheCategorySchedule()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var scheduleId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("Working hours"))).Content;

        var recurrence = await api.Categories.CreateRecurrence(
            TestRequests.NewSchedule(scheduleId, TestRequests.EveryNDays(1)));
        recurrence.StatusCode.Should().Be(HttpStatusCode.Created);

        // The link lives on the window, not on the category.
        var stored = await AdminDbContext.Set<CategorySchedule>().SingleAsync(cancellationToken);
        stored.ScheduleEntityId.Should().Be(recurrence.Content!.Id);

        // And it must come back nested on the category, which is what the edit form reads.
        var get = await api.Categories.Get(categoryId);
        get.Content!.Schedules.Should().ContainSingle()
            .Which.ScheduleEntity!.Id.Should().Be(recurrence.Content.Id);
    }

    [Fact, Trait("Endpoint", "POST /api/categories/schedules")]
    public async Task CreateRecurrence_Should_AnchorProgressMarkersToTheWindowsDate()
    {
        var (api, categoryId) = await CreateCategoryAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var date = Today.AddDays(5);

        var scheduleId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("Working hours", date))).Content;
        await api.Categories.CreateRecurrence(TestRequests.NewSchedule(scheduleId, TestRequests.EveryNDays(1)));

        // The window already occupies its own date, so the recurrence must resume after it.
        var stored = await AdminDbContext.Set<ScheduleEntity>().AsNoTracking().SingleAsync(cancellationToken);
        stored.FirstEntityCreated.Should().Be(date);
        stored.LastEntityCreated.Should().Be(date);
    }

    [Fact, Trait("Endpoint", "POST /api/categories/schedules")]
    public async Task CreateRecurrence_OnSpecificDates_Should_DeriveEndsOnFromLastDate()
    {
        var (api, categoryId) = await CreateCategoryAsync("Workshop");
        var scheduleId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule())).Content;
        // Relative to today: chosen dates must fall after the window's own date, which is today.
        var first = Today.AddDays(7);
        var last = first.AddDays(17);

        var recurrence = await api.Categories.CreateRecurrence(
            TestRequests.NewSchedule(scheduleId, TestRequests.OnDates(last, first)));

        recurrence.StatusCode.Should().Be(HttpStatusCode.Created);
        // A finite list of dates is self-bounding, so the server derives EndsOn instead of taking it.
        recurrence.Content!.EndsOn.Should().Be(last);
    }

    [Theory]
    [InlineData(0)]   // the window's own date (today)
    [InlineData(-1)]  // yesterday
    [Trait("Endpoint", "Validation")]
    public async Task CreateRecurrence_OnSpecificDates_Should_Return400_WhenDateNotAfterAnchor(int offsetFromToday)
    {
        var (api, categoryId) = await CreateCategoryAsync("Workshop");

        // The window is anchored to today and the series only walks forward, so these dates could
        // never produce an occurrence.
        var scheduleId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule())).Content;
        var unreachable = Today.AddDays(offsetFromToday);

        var recurrence = await api.Categories.CreateRecurrence(
            TestRequests.NewSchedule(scheduleId, TestRequests.OnDates(unreachable)));

        recurrence.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task CreateRecurrence_Should_Return404_ForUnknownCategorySchedule()
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Categories.CreateRecurrence(
            TestRequests.NewSchedule(Guid.CreateVersion7(), TestRequests.EveryNDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.GetProperty("ResourceName").GetString().Should().Be("CategorySchedule");
    }

    // --- RLS isolation ---

    [Fact, Trait("Security", "RLS isolation")]
    public async Task Create_ForAnotherUsersCategory_Should_Return404()
    {
        var (_, categoryId) = await CreateCategoryAsync("A-only");

        var userB = await CreateAuthenticatedApiAsync();
        var response = await userB.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Trait("Security", "RLS isolation")]
    public async Task Update_Delete_ForAnotherUsersSchedule_Should_Return404()
    {
        var (userA, categoryId) = await CreateCategoryAsync("A-only");
        var scheduleId = (await userA.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("A's window"))).Content;

        var userB = await CreateAuthenticatedApiAsync();

        (await userB.Categories.UpdateSchedule(categoryId, scheduleId, TestRequests.NewCategorySchedule("Hacked")))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await userB.Categories.DeleteSchedule(categoryId, scheduleId))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);

        // A's window is untouched — a cross-user write is a 404, never a silent no-op.
        (await userA.Categories.GetSchedules(categoryId)).Content!
            .Should().ContainSingle().Which.Description.Should().Be("A's window");
    }

    [Fact, Trait("Security", "RLS isolation")]
    public async Task GetSchedules_ForAnotherUsersCategory_Should_BeEmpty()
    {
        var (userA, categoryId) = await CreateCategoryAsync("A-only");
        await userA.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule("A's window"));

        var userB = await CreateAuthenticatedApiAsync();
        var response = await userB.Categories.GetSchedules(categoryId);

        // A streaming read is not a lookup, so RLS shows B an empty list rather than a 404.
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content!.Should().BeEmpty();
    }

    [Fact, Trait("Security", "RLS isolation")]
    public async Task CreateRecurrence_ForAnotherUsersSchedule_Should_Return404()
    {
        var (userA, categoryId) = await CreateCategoryAsync("A-only");
        var scheduleId = (await userA.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule())).Content;

        var userB = await CreateAuthenticatedApiAsync();
        var response = await userB.Categories.CreateRecurrence(
            TestRequests.NewSchedule(scheduleId, TestRequests.EveryNDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
