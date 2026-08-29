using System.Drawing;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Integration.Api.Tests.Categories;

public sealed class CategoriesApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    [Fact, Trait("Endpoint", "POST+GET /api/categories")]
    public async Task Create_Should_PersistAndRoundTrip()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        var create = await api.Categories.Create(TestRequests.NewCategory("Work", Color.Teal, "desc"));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var get = await api.Categories.Get(create.Content);
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        get.Content!.Name.Should().Be("Work");
        get.Content.Description.Should().Be("desc");
        get.Content.Color.ToArgb().Should().Be(Color.Teal.ToArgb());

        (await AdminDbContext.Set<Category>().CountAsync(cancellationToken)).Should().Be(1);
    }

    [Fact, Trait("Endpoint", "GET /api/categories")]
    public async Task GetAll_Should_StreamAllOwnedCategories()
    {
        var api = await CreateAuthenticatedApiAsync();

        await api.Categories.Create(TestRequests.NewCategory("A"));
        await api.Categories.Create(TestRequests.NewCategory("B"));
        await api.Categories.Create(TestRequests.NewCategory("C"));

        var all = await api.Categories.GetAll();
        all.StatusCode.Should().Be(HttpStatusCode.OK);
        all.Content!.Select(c => c.Name).Should().BeEquivalentTo("A", "B", "C");
    }

    [Fact, Trait("Endpoint", "PUT /api/categories/{id}")]
    public async Task Update_Should_ChangeNameAndColor()
    {
        var api = await CreateAuthenticatedApiAsync();

        var id = (await api.Categories.Create(TestRequests.NewCategory("Old", Color.Red))).Content;

        var update = await api.Categories.Update(id, TestRequests.NewCategory("New", Color.Green, "updated"));
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var get = await api.Categories.Get(id);
        get.Content!.Name.Should().Be("New");
        get.Content.Description.Should().Be("updated");
        get.Content.Color.ToArgb().Should().Be(Color.Green.ToArgb());
    }

    [Fact, Trait("Endpoint", "DELETE /api/categories/{id}")]
    public async Task Delete_Should_Return204_AndRemoveRow()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        var id = (await api.Categories.Create(TestRequests.NewCategory("Temp"))).Content;

        var delete = await api.Categories.Delete(id);
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await api.Categories.Get(id)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await AdminDbContext.Set<Category>().CountAsync(cancellationToken)).Should().Be(0);
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Get_Update_Delete_Should_Return404_ForUnknownId()
    {
        var api = await CreateAuthenticatedApiAsync();
        var unknown = Guid.CreateVersion7();

        (await api.Categories.Get(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.Categories.Update(unknown, TestRequests.NewCategory())).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.Categories.Delete(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Trait("Endpoint", "Validation")]
    public async Task Create_Should_Return400_WhenNameEmpty()
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Categories.Create(TestRequests.NewCategory(name: ""));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Security", "RLS isolation")]
    public async Task Get_ByAnotherUser_Should_Return404()
    {
        var userA = await CreateAuthenticatedApiAsync();
        var id = (await userA.Categories.Create(TestRequests.NewCategory("A-only"))).Content;

        var userB = await CreateAuthenticatedApiAsync(); // different real user (own auth cookie)
        var response = await userB.Categories.Get(id);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // RLS hides A's row from B on reads too
    }

    [Fact, Trait("Endpoint", "POST+GET /api/categories")]
    public async Task Create_Should_RoundTripTimeWindow()
    {
        var api = await CreateAuthenticatedApiAsync();

        var create = await api.Categories.Create(
            TestRequests.NewCategory(startTime: new TimeOnly(09, 30), endTime: new TimeOnly(17, 45)));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var get = await api.Categories.Get(create.Content);
        get.Content!.StartTime.Should().Be(new TimeOnly(09, 30));
        get.Content.EndTime.Should().Be(new TimeOnly(17, 45));
    }

    [Fact, Trait("Endpoint", "POST+GET /api/categories")]
    public async Task Create_Should_RoundTripDate()
    {
        var api = await CreateAuthenticatedApiAsync();
        var date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3);

        var create = await api.Categories.Create(TestRequests.NewCategory(date: date));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var get = await api.Categories.Get(create.Content);
        get.Content!.Date.Should().Be(date);
    }

    [Fact, Trait("Endpoint", "Validation")]
    public async Task Create_Should_Return400_WhenEndTimeNotAfterStartTime()
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Categories.Create(
            TestRequests.NewCategory(startTime: new TimeOnly(18, 00), endTime: new TimeOnly(09, 00)));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Endpoint", "POST /api/categories/schedules")]
    public async Task CreateSchedule_Should_LinkScheduleToCategory()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Work"))).Content;

        var schedule = await api.Categories.CreateSchedule(
            TestRequests.NewSchedule(categoryId, TestRequests.EveryNDays(1)));

        schedule.StatusCode.Should().Be(HttpStatusCode.Created);

        var stored = await AdminDbContext.Set<Category>().SingleAsync(cancellationToken);
        stored.ScheduleEntityId.Should().Be(schedule.Content!.Id);

        // The schedule must also come back on the category itself, which is what the edit form reads.
        var get = await api.Categories.Get(categoryId);
        get.Content!.ScheduleEntity.Should().NotBeNull();
        get.Content.ScheduleEntity!.Id.Should().Be(schedule.Content.Id);
    }

    [Fact, Trait("Endpoint", "POST /api/categories/schedules")]
    public async Task CreateSchedule_OnSpecificDates_Should_DeriveEndsOnFromLastDate()
    {
        var api = await CreateAuthenticatedApiAsync();

        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Workshop"))).Content;
        // Relative to today: chosen dates must fall after the category's own date, which is today.
        var first = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7);
        var last = first.AddDays(17);

        var schedule = await api.Categories.CreateSchedule(
            TestRequests.NewSchedule(categoryId, TestRequests.OnDates(last, first)));

        schedule.StatusCode.Should().Be(HttpStatusCode.Created);
        // A finite list of dates is self-bounding, so the server derives EndsOn instead of taking it.
        schedule.Content!.EndsOn.Should().Be(last);
    }

    [Theory]
    [InlineData(0)]   // the category's own date (today)
    [InlineData(-1)]  // yesterday
    [Trait("Endpoint", "Validation")]
    public async Task CreateSchedule_OnSpecificDates_Should_Return400_WhenDateNotAfterAnchor(int offsetFromToday)
    {
        var api = await CreateAuthenticatedApiAsync();

        // The category is anchored to today and the series only walks forward, so these dates could
        // never produce an occurrence.
        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Workshop"))).Content;
        var unreachable = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(offsetFromToday);

        var schedule = await api.Categories.CreateSchedule(
            TestRequests.NewSchedule(categoryId, TestRequests.OnDates(unreachable)));

        schedule.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Endpoint", "POST /api/categories/schedules")]
    public async Task CreateSchedule_Should_AnchorProgressMarkersToCategoryDate()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5);

        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Work", date: date))).Content;
        await api.Categories.CreateSchedule(TestRequests.NewSchedule(categoryId, TestRequests.EveryNDays(1)));

        // The category already occupies its own date, so the recurrence must resume after it.
        var stored = await AdminDbContext.Set<ScheduleEntity>().AsNoTracking().SingleAsync(cancellationToken);
        stored.FirstEntityCreated.Should().Be(date);
        stored.LastEntityCreated.Should().Be(date);
    }

    [Fact, Trait("Endpoint", "PUT /api/categories/{id}")]
    public async Task Update_Should_KeepAttachedSchedule()
    {
        var api = await CreateAuthenticatedApiAsync();

        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Work"))).Content;
        var scheduleId = (await api.Categories.CreateSchedule(
            TestRequests.NewSchedule(categoryId, TestRequests.EveryNDays(1)))).Content!.Id;

        // The edit payload carries no schedule link, so writing it back would silently unlink the recurrence.
        await api.Categories.Update(categoryId, TestRequests.NewCategory("Work renamed"));

        var get = await api.Categories.Get(categoryId);
        get.Content!.Name.Should().Be("Work renamed");
        get.Content.ScheduleEntity!.Id.Should().Be(scheduleId);
    }

    [Fact, Trait("Security", "RLS isolation")]
    public async Task CreateSchedule_ForAnotherUsersCategory_Should_Return404()
    {
        var userA = await CreateAuthenticatedApiAsync();
        var categoryId = (await userA.Categories.Create(TestRequests.NewCategory("A-only"))).Content;

        var userB = await CreateAuthenticatedApiAsync();
        var response = await userB.Categories.CreateSchedule(
            TestRequests.NewSchedule(categoryId, TestRequests.EveryNDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
