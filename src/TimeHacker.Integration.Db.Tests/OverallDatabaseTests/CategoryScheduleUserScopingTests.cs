using TimeHacker.Domain.BusinessLogicExceptions;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

/// <summary>
/// CategorySchedule is a new user-scoped table, so it needs the same two-way proof as every other one:
/// Full (through the repository, which stamps UserId and rejects violations in-app) and RLS only
/// (through a raw context with app.user_id set, exercising the PostgreSQL policy directly). Without the
/// policy every read here would come back empty instead of scoped, so these also prove the migration
/// actually created it.
/// </summary>
public class CategoryScheduleUserScopingTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("GetAll", "UserScoping")]
    public async Task GetAll_Should_OnlyReturnCurrentUsersSchedules()
    {
        var ct = TestContext.Current.CancellationToken;
        var own = await Resolve<GraphSeeder>().SeedCategoryScheduleForCurrentUser("Mine");
        foreach (var user in OtherUsers)
            await user.Resolve<GraphSeeder>().SeedCategoryScheduleForCurrentUser("Theirs");

        // Full: the repository returns only the current user's window.
        var result = await Resolve<ICategoryScheduleRepository>().GetAll().ToListAsync(ct);
        result.Should().ContainSingle().Which.Id.Should().Be(own.Id);

        (await Db.Set<CategorySchedule>().CountAsync(ct)).Should().Be(OtherUsers.Count + 1);

        // RLS only: a raw Set<T>() query with no repository filter still sees only the current user's row.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        var rlsResult = await rls.Set<CategorySchedule>().ToListAsync(ct);
        rlsResult.Should().ContainSingle().Which.UserId.Should().Be(CurrentUser.UserId);
    }

    [Fact]
    [Trait("GetByIdAsync", "UserScoping")]
    public async Task GetByIdAsync_Should_NotReturnAnotherUsersSchedule()
    {
        var ct = TestContext.Current.CancellationToken;
        var theirs = await SeedForOtherUser();

        (await Resolve<ICategoryScheduleRepository>().GetByIdAsync(theirs.Id, cancellationToken: ct)).Should().BeNull();

        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        (await rls.Set<CategorySchedule>().FirstOrDefaultAsync(c => c.Id == theirs.Id, ct)).Should().BeNull();
    }

    [Fact]
    [Trait("ExistsAsync", "UserScoping")]
    public async Task ExistsAsync_Should_BeScopedToCurrentUser()
    {
        var ct = TestContext.Current.CancellationToken;
        var theirs = await SeedForOtherUser();
        var own = await Resolve<GraphSeeder>().SeedCategoryScheduleForCurrentUser();

        var repo = Resolve<ICategoryScheduleRepository>();
        (await repo.ExistsAsync(theirs.Id, ct)).Should().BeFalse();
        (await repo.ExistsAsync(own.Id, ct)).Should().BeTrue();

        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        (await rls.Set<CategorySchedule>().AnyAsync(c => c.Id == theirs.Id, ct)).Should().BeFalse();
        (await rls.Set<CategorySchedule>().AnyAsync(c => c.Id == own.Id, ct)).Should().BeTrue();
    }

    [Fact]
    [Trait("Add", "UserScoping")]
    public async Task Add_Should_RejectForeignUserIdUnderRls()
    {
        var ct = TestContext.Current.CancellationToken;
        var own = await Resolve<GraphSeeder>().SeedCategoryScheduleForCurrentUser();

        // RLS only: inserting a row stamped for another user violates the WITH CHECK policy.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        rls.Set<CategorySchedule>().Add(new CategorySchedule
        {
            Id = Guid.NewGuid(),
            UserId = OtherUsers.First().UserId,
            CategoryId = own.CategoryId,
            Description = "Smuggled",
            Date = own.Date,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(10, 0)
        });

        var act = async () => await rls.SaveChangesAsync(ct);
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    [Trait("Update", "UserScoping")]
    public async Task Update_Should_RejectAnotherUsersSchedule()
    {
        var ct = TestContext.Current.CancellationToken;
        var theirs = await SeedForOtherUser();
        var repo = Resolve<ICategoryScheduleRepository>();

        // Full: updating another user's window is rejected in-app.
        theirs.Description = "Hacked";
        var act = async () => await repo.UpdateAndSaveAsync(theirs, ct);
        await act.Should().ThrowAsync<NotFoundException>();

        // RLS only: a raw update of an invisible row affects 0 rows -> concurrency conflict.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        var stub = new CategorySchedule { Id = theirs.Id, UserId = CurrentUser.UserId, Description = "x" };
        rls.Attach(stub);
        rls.Entry(stub).Property(c => c.Description).IsModified = true;
        var rlsAct = async () => await rls.SaveChangesAsync(ct);
        await rlsAct.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    [Fact]
    [Trait("Update", "UserScoping")]
    public async Task Update_Should_RejectChangingUserIdToAnotherUser()
    {
        var ct = TestContext.Current.CancellationToken;
        var own = await Resolve<GraphSeeder>().SeedCategoryScheduleForCurrentUser();

        // RLS only: reassigning an owned row to another user violates the WITH CHECK policy.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        var loaded = await rls.Set<CategorySchedule>().SingleAsync(c => c.Id == own.Id, ct);
        loaded.UserId = OtherUsers.First().UserId;

        var act = async () => await rls.SaveChangesAsync(ct);
        await act.Should().ThrowAsync<DbUpdateException>();

        (await Db.Set<CategorySchedule>().AsNoTracking().SingleAsync(c => c.Id == own.Id, ct))
            .UserId.Should().Be(CurrentUser.UserId);
    }

    [Fact]
    [Trait("Delete", "UserScoping")]
    public async Task Delete_Should_RejectAnotherUsersSchedule()
    {
        var ct = TestContext.Current.CancellationToken;
        var theirs = await SeedForOtherUser();
        var repo = Resolve<ICategoryScheduleRepository>();

        // Full: deleting another user's window via a tracked model is rejected in-app.
        var act = async () => await repo.DeleteAndSaveAsync(theirs, ct);
        await act.Should().ThrowAsync<NotFoundException>();

        // The id-based delete can't see the row, so it's a no-op returning false.
        (await repo.DeleteAndSaveAsync(theirs.Id, ct)).Should().BeFalse();
        (await Db.Set<CategorySchedule>().AsNoTracking().AnyAsync(c => c.Id == theirs.Id, ct)).Should().BeTrue();

        // RLS only: a raw delete of an invisible row affects 0 rows -> concurrency conflict.
        await using (var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct))
        {
            rls.Remove(new CategorySchedule { Id = theirs.Id, UserId = CurrentUser.UserId });
            var rlsAct = async () => await rls.SaveChangesAsync(ct);
            await rlsAct.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }

        // The owner can still delete its own row.
        var ownerRepo = OtherUsers.First(u => u.UserId == theirs.UserId).Resolve<ICategoryScheduleRepository>();
        (await ownerRepo.DeleteAndSaveAsync(theirs.Id, ct)).Should().BeTrue();
        (await Db.Set<CategorySchedule>().AsNoTracking().AnyAsync(c => c.Id == theirs.Id, ct)).Should().BeFalse();
    }

    [Fact]
    [Trait("Add", "SameDayWindows")]
    public async Task Add_Should_AllowSeveralOverlappingWindowsOnOneDayForOneCategory()
    {
        var ct = TestContext.Current.CancellationToken;
        var date = new DateOnly(2026, 6, 1);

        var (category, morning, evening) = await Resolve<GraphSeeder>().SeedCategoryWithTwoWindowsOn(date, ct);

        // The (UserId, Date) index is a plain lookup index — nothing constrains one window per day, and
        // a third window overlapping the first must insert cleanly too.
        var overlapping = await Resolve<ICategoryScheduleRepository>().AddAndSaveAsync(new CategorySchedule
        {
            CategoryId = category.Id,
            Description = "Meetings",
            Date = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0)
        }, ct);

        var stored = await Db.Set<CategorySchedule>().AsNoTracking()
            .Where(x => x.CategoryId == category.Id && x.Date == date)
            .ToListAsync(ct);

        stored.Select(x => x.Id).Should().BeEquivalentTo([morning.Id, evening.Id, overlapping.Id]);
    }

    private Task<CategorySchedule> SeedForOtherUser()
        => OtherUsers.First().Resolve<GraphSeeder>().SeedCategoryScheduleForCurrentUser("Theirs");
}
