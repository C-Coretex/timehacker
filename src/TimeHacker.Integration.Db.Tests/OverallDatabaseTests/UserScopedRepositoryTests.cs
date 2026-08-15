using TimeHacker.Domain.BusinessLogicExceptions;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

// Every user-scoping guarantee is verified two ways:
//   * Full  - through the repository, which stamps UserId and rejects violations in-app (NotFoundException).
//   * RLS only - through a raw TimeHackerDbContext with app.user_id set (no repository, no interceptor,
//     no in-app guard), which exercises the PostgreSQL policies directly (filtered reads, DbUpdateException
//     on WITH CHECK violations, DbUpdateConcurrencyException when a cross-user write hits 0 visible rows).
public class UserScopedRepositoryTests(DbContainerFixture fixture): DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("GetAll", "UserScoping")]
    public async Task GetAll_Should_OnlyReturnCurrentUsersCategories()
    {
        var ct = TestContext.Current.CancellationToken;
        var seedDataService = Resolve<SeedDataBuilder<ICategoryRepository, Category, Guid>>();
        var repo = await seedDataService.SeedUsersData(OtherUsers);

        // Full: repository GetAll returns only the current user's row.
        var result = await repo.GetAll().ToListAsync(ct);
        result.Should().ContainSingle().Which.UserId.Should().Be(CurrentUser.UserId);

        var otherUsersResult = await OtherUsers.First().Resolve<ICategoryRepository>().GetAll().ToListAsync(ct);
        otherUsersResult.Should().ContainSingle().Which.UserId.Should().Be(OtherUsers.First().UserId);

        var allEntries = await Db.Set<Category>().ToListAsync(ct);
        allEntries.Should().HaveCount(OtherUsers.Count + 1);
        allEntries.Should().ContainSingle(c => c.UserId == CurrentUser.UserId);
        foreach (var user in OtherUsers)
            allEntries.Should().ContainSingle(c => c.UserId == user.UserId);

        // RLS only: a raw Set<T>() query (no repository .Where filter) still sees only the current user's row.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        var rlsResult = await rls.Set<Category>().ToListAsync(ct);
        rlsResult.Should().ContainSingle().Which.UserId.Should().Be(CurrentUser.UserId);
    }

    [Theory]
    [InlineData(true), InlineData(false)]
    [Trait("Add", "UserScoping")]
    public async Task Add_Should_CreateTaskForCurrentUser(bool useCurrentUser)
    {
        var seedDataService = Resolve<SeedDataBuilder<IFixedTaskRepository, FixedTask, Guid>>();
        var userRepo = useCurrentUser ? Resolve<IFixedTaskRepository>() : OtherUsers.First().Resolve<IFixedTaskRepository>();

        var task = new FixedTask
        {
            UserId =  Guid.NewGuid(),
            Name = "Test Task",
            Description = "Test Description",
            Priority = 1,
            StartTimestamp = DateTime.UtcNow,
            EndTimestamp = DateTime.UtcNow.AddHours(1)
        };
        await userRepo.AddAndSaveAsync(task, TestContext.Current.CancellationToken);

        var currentUserRepo = Resolve<IFixedTaskRepository>();
        var result = await currentUserRepo.GetAll().ToListAsync(TestContext.Current.CancellationToken);

        result.Should().HaveCount(useCurrentUser ? 1 : 0);
        if(useCurrentUser)
            result.Should().ContainSingle().Which.UserId.Should().Be(CurrentUser.UserId);

        // RLS only: add with a different user ID throws an exception.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, TestContext.Current.CancellationToken);
        rls.Set<Category>().Add(new Category { Id = Guid.NewGuid(), UserId = OtherUsers.First().UserId });
        var rlsAct = async () => await rls.SaveChangesAsync(TestContext.Current.CancellationToken);
        await rlsAct.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    [Trait("GetByIdAsync", "UserScoping")]
    public async Task GetByIdAsync_Should_NotReturnAnotherUsersEntity()
    {
        var ct = TestContext.Current.CancellationToken;
        var otherUsersCategory = await SeedCategoryForOtherUser();

        // Full: repository GetById can't see another user's row.
        var repo = Resolve<ICategoryRepository>();
        (await repo.GetByIdAsync(otherUsersCategory.Id, cancellationToken: ct)).Should().BeNull();

        // RLS only: a raw query for the same id returns nothing.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        (await rls.Set<Category>().FirstOrDefaultAsync(c => c.Id == otherUsersCategory.Id, ct)).Should().BeNull();
    }

    [Fact]
    [Trait("ExistsAsync", "UserScoping")]
    public async Task ExistsAsync_Should_BeScopedToCurrentUser()
    {
        var ct = TestContext.Current.CancellationToken;
        var otherUsersCategory = await SeedCategoryForOtherUser();
        var ownCategory = await Resolve<SeedDataBuilder<ICategoryRepository, Category, Guid>>().SeedForCurrentUser();

        // Full: repository ExistsAsync is scoped to the current user.
        var repo = Resolve<ICategoryRepository>();
        (await repo.ExistsAsync(otherUsersCategory.Id, ct)).Should().BeFalse();
        (await repo.ExistsAsync(ownCategory.Id, ct)).Should().BeTrue();

        // RLS only: a raw existence check sees the own row but not the other user's.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        (await rls.Set<Category>().AnyAsync(c => c.Id == otherUsersCategory.Id, ct)).Should().BeFalse();
        (await rls.Set<Category>().AnyAsync(c => c.Id == ownCategory.Id, ct)).Should().BeTrue();
    }

    [Fact]
    [Trait("Add", "UserScoping")]
    public async Task Add_Should_OverwriteProvidedForeignUserIdWithCurrentUser()
    {
        var ct = TestContext.Current.CancellationToken;
        var repo = Resolve<IFixedTaskRepository>();

        // Full: the caller claims the row belongs to another user; SaveChanges stamps the current user.
        var task = new FixedTask
        {
            UserId = OtherUsers.First().UserId,
            Name = "Test Task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)
        };

        var created = await repo.AddAndSaveAsync(task, ct);
        created.UserId.Should().Be(CurrentUser.UserId);

        var persisted = await Db.Set<FixedTask>().SingleAsync(t => t.Id == created.Id, ct);
        persisted.UserId.Should().Be(CurrentUser.UserId);

        // RLS only: a raw insert claiming another user's id is rejected by the WITH CHECK policy.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        rls.Set<FixedTask>().Add(new FixedTask
        {
            UserId = OtherUsers.First().UserId,
            Name = "Foreign Task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)
        });
        var rlsAct = async () => await rls.SaveChangesAsync(ct);
        await rlsAct.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    [Trait("Update", "UserScoping")]
    public async Task Update_Should_RejectAnotherUsersEntity()
    {
        var ct = TestContext.Current.CancellationToken;
        var otherUsersCategory = await SeedCategoryForOtherUser();
        var otherUsersCategory2 = await SeedCategoryForOtherUser();
        var repo = Resolve<ICategoryRepository>();

        // Full: updating another user's row (full object or partial stub) is rejected in-app.
        otherUsersCategory.Description = "Updated Description";
        var act = async () => await repo.UpdateAndSaveAsync(otherUsersCategory, ct);
        await act.Should().ThrowAsync<NotFoundException>();

        // A second (different-key) seed avoids an EF identity conflict in the shared context.
        act = async () => await repo.UpdateAndSaveAsync(new() { Id = otherUsersCategory2.Id, Description = "Updated Description" }, ct);
        await act.Should().ThrowAsync<NotFoundException>();

        // RLS only: a raw update of an invisible row affects 0 rows -> concurrency conflict.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        var stub = new Category { Id = otherUsersCategory.Id, UserId = CurrentUser.UserId, Description = "x" };
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
        var category = await Resolve<SeedDataBuilder<ICategoryRepository, Category, Guid>>().SeedForCurrentUser();
        var repo = Resolve<ICategoryRepository>();

        // Full: reassigning an owned row to another user is rejected in-app, right away.
        category.UserId = OtherUsers.First().UserId;
        var act = async () => await repo.UpdateAndSaveAsync(category, ct);
        await act.Should().ThrowAsync<NotFoundException>();

        (await Db.Set<Category>().AsNoTracking().SingleAsync(c => c.Id == category.Id, ct))
            .UserId.Should().Be(CurrentUser.UserId);

        // A normal update on the tracked, owned row still succeeds.
        category.UserId = CurrentUser.UserId;
        category.Description = "new desc";
        await repo.UpdateAndSaveAsync(category, ct);

        var persisted = await Db.Set<Category>().AsNoTracking().SingleAsync(c => c.Id == category.Id, ct);
        persisted.UserId.Should().Be(CurrentUser.UserId);
        persisted.Description.Should().Be("new desc");

        // RLS only: loading the owned row and reassigning UserId violates the WITH CHECK policy.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        var own = await rls.Set<Category>().SingleAsync(c => c.Id == category.Id, ct);
        own.UserId = OtherUsers.First().UserId;
        var rlsAct = async () => await rls.SaveChangesAsync(ct);
        await rlsAct.Should().ThrowAsync<DbUpdateException>();

        (await Db.Set<Category>().AsNoTracking().SingleAsync(c => c.Id == category.Id, ct))
            .UserId.Should().Be(CurrentUser.UserId);
    }

    [Fact]
    [Trait("Delete", "UserScoping")]
    public async Task Delete_Should_RejectAnotherUsersEntity()
    {
        var ct = TestContext.Current.CancellationToken;
        var otherUsersCategory = await SeedCategoryForOtherUser();
        var repo = Resolve<ICategoryRepository>();

        // Full: deleting another user's row via a tracked model is rejected in-app.
        var act = async () => await repo.DeleteAndSaveAsync(otherUsersCategory, ct);
        await act.Should().ThrowAsync<NotFoundException>();

        // The id-based bulk delete can't see the row, so it's a no-op returning false.
        (await repo.DeleteAndSaveAsync(otherUsersCategory.Id, ct)).Should().BeFalse();

        (await Db.Set<Category>().AsNoTracking().AnyAsync(c => c.Id == otherUsersCategory.Id, ct)).Should().BeTrue();

        // RLS only: a raw delete of an invisible row affects 0 rows -> concurrency conflict.
        await using (var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct))
        {
            rls.Remove(new Category { Id = otherUsersCategory.Id, UserId = CurrentUser.UserId });
            var rlsAct = async () => await rls.SaveChangesAsync(ct);
            await rlsAct.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }

        // The owner can still delete its own row.
        var otherUsersRepo = OtherUsers.First(u => u.UserId == otherUsersCategory.UserId).Resolve<ICategoryRepository>();
        await otherUsersRepo.DeleteAndSaveAsync(otherUsersCategory.Id, ct);
        (await Db.Set<Category>().AsNoTracking().AnyAsync(c => c.Id == otherUsersCategory.Id, ct)).Should().BeFalse();
    }

    [Fact]
    [Trait("Add", "UserScoping")]
    public async Task Add_Should_OverwriteForeignUserIdOnNavigationChild()
    {
        var ct = TestContext.Current.CancellationToken;
        var otherUserId = OtherUsers.First().UserId;

        // Full: a task owned by the current user, but whose nested ScheduleEntity (incorrectly) claims
        // another user. ScheduleEntity.UserId is not part of any FK to the task, so only the repository's
        // SaveChanges stamping can correct it - EF relationship fixup won't.
        var task = new FixedTask
        {
            Name = "Task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
            ScheduleEntity = new ScheduleEntity { UserId = otherUserId, RepeatingEntity = GraphSeeder.DailyRepeat() }
        };

        await Resolve<IFixedTaskRepository>().AddAndSaveAsync(task, ct);

        Db.ChangeTracker.Clear();
        var persistedScheduleEntity = await Db.Set<ScheduleEntity>().SingleAsync(ct);
        persistedScheduleEntity.UserId.Should().Be(CurrentUser.UserId);

        // RLS only: a raw insert whose child claims another user is rejected by the WITH CHECK policy.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        rls.Set<FixedTask>().Add(new FixedTask
        {
            UserId = CurrentUser.UserId,
            Name = "Task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
            ScheduleEntity = new ScheduleEntity { UserId = otherUserId, RepeatingEntity = GraphSeeder.DailyRepeat() }
        });
        var rlsAct = async () => await rls.SaveChangesAsync(ct);
        await rlsAct.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    [Trait("Update", "UserScoping")]
    public async Task Update_Should_RejectForeignUserIdOnNavigationChild()
    {
        var ct = TestContext.Current.CancellationToken;

        // A schedule entity that genuinely belongs to another user.
        var foreignSchedule = await SeedScheduleForOtherUser();

        // Full: a task graph for the current user whose schedule child is a stub pointing at another
        // user's row, marked Modified. The foreign row is invisible under RLS, so the UPDATE affects 0
        // rows and is mapped to NotFound for the current user.
        var dbContext = Resolve<TimeHackerDbContext>();
        var scheduleRepo = Resolve<IScheduleEntityRepository>();

        var task = GraphSeeder.BuildTaskWithScheduleStub(foreignSchedule.Id, CurrentUser.UserId);
        dbContext.Attach(task);
        dbContext.Entry(task.ScheduleEntity!).State = EntityState.Modified;

        var act = async () => await scheduleRepo.SaveChangesAsync(ct);
        await act.Should().ThrowAsync<NotFoundException>();

        // RLS only: the same stub update via a raw context hits 0 visible rows -> concurrency conflict.
        await using var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct);
        var rlsTask = GraphSeeder.BuildTaskWithScheduleStub(foreignSchedule.Id, CurrentUser.UserId);
        rls.Attach(rlsTask);
        rls.Entry(rlsTask.ScheduleEntity!).State = EntityState.Modified;
        var rlsAct = async () => await rls.SaveChangesAsync(ct);
        await rlsAct.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    [Fact]
    [Trait("Delete", "UserScoping")]
    public async Task Delete_Should_RejectForeignUserIdOnNavigationChild()
    {
        var ct = TestContext.Current.CancellationToken;
        var foreignSchedule = await SeedScheduleForOtherUser();

        // Full: same stub graph, but the foreign schedule child is marked Deleted. Deleting a row owned
        // by another user hits 0 rows under RLS -> NotFound.
        var dbContext = Resolve<TimeHackerDbContext>();
        var scheduleRepo = Resolve<IScheduleEntityRepository>();

        var task = GraphSeeder.BuildTaskWithScheduleStub(foreignSchedule.Id, CurrentUser.UserId);
        dbContext.Attach(task);
        dbContext.Entry(task.ScheduleEntity!).State = EntityState.Deleted;

        var act = async () => await scheduleRepo.SaveChangesAsync(ct);
        await act.Should().ThrowAsync<NotFoundException>();

        // RLS only: the same stub delete via a raw context hits 0 visible rows -> concurrency conflict.
        await using (var rls = await CreateRlsContextAsync(CurrentUser.UserId, ct))
        {
            var rlsTask = GraphSeeder.BuildTaskWithScheduleStub(foreignSchedule.Id, CurrentUser.UserId);
            rls.Attach(rlsTask);
            rls.Entry(rlsTask.ScheduleEntity!).State = EntityState.Deleted;
            var rlsAct = async () => await rls.SaveChangesAsync(ct);
            await rlsAct.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }

        // The other user's row must still be present.
        (await Db.Set<ScheduleEntity>().AsNoTracking().AnyAsync(s => s.Id == foreignSchedule.Id, ct)).Should().BeTrue();
    }

    private Task<Category> SeedCategoryForOtherUser()
        => OtherUsers.First().Resolve<SeedDataBuilder<ICategoryRepository, Category, Guid>>().SeedForCurrentUser();

    private Task<ScheduleEntity> SeedScheduleForOtherUser()
        => OtherUsers.First().Resolve<SeedDataBuilder<IScheduleEntityRepository, ScheduleEntity, Guid>>()
            .SeedForCurrentUser();
}
