using Microsoft.EntityFrameworkCore;
using Timehacker.Integration.Db.Tests;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Infrastructure;
using TimeHacker.Integration.Db.Tests.Fixtures;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

public class UserScopedRepositoryTests(DbContainerFixture fixture): DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("GetAll", "UserScoping")]
    public async Task GetAll_Should_OnlyReturnCurrentUsersCategories()
    {
        var seedDataService = Resolve<SeedDataBuilder<ICategoryRepository, Category, Guid>>();
        var repo = await seedDataService.SeedUsersData(OtherUsers);

        var result = await repo.GetAll().ToListAsync(TestContext.Current.CancellationToken);

        result.Should().ContainSingle().Which.UserId.Should().Be(CurrentUser.UserId);

        var otherUsersResult = await OtherUsers.First().Resolve<ICategoryRepository>().GetAll().ToListAsync(TestContext.Current.CancellationToken);

        otherUsersResult.Should().ContainSingle().Which.UserId.Should().Be(OtherUsers.First().UserId);

        var allEntries = await Db.Set<Category>().ToListAsync(TestContext.Current.CancellationToken);

        allEntries.Should().HaveCount(OtherUsers.Count + 1);
        allEntries.Should().ContainSingle(c => c.UserId == CurrentUser.UserId);
        foreach(var user in OtherUsers)
            allEntries.Should().ContainSingle(c => c.UserId == user.UserId);
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
    }

    [Fact]
    [Trait("GetByIdAsync", "UserScoping")]
    public async Task GetByIdAsync_Should_NotReturnAnotherUsersEntity()
    {
        var otherUsersCategory = await SeedCategoryForOtherUser();
        var repo = Resolve<ICategoryRepository>();

        var result = await repo.GetByIdAsync(otherUsersCategory.Id, cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    [Trait("ExistsAsync", "UserScoping")]
    public async Task ExistsAsync_Should_BeScopedToCurrentUser()
    {
        var otherUsersCategory = await SeedCategoryForOtherUser();
        var ownCategory = await Resolve<SeedDataBuilder<ICategoryRepository, Category, Guid>>().SeedForCurrentUser();
        var repo = Resolve<ICategoryRepository>();

        (await repo.ExistsAsync(otherUsersCategory.Id, TestContext.Current.CancellationToken)).Should().BeFalse();
        (await repo.ExistsAsync(ownCategory.Id, TestContext.Current.CancellationToken)).Should().BeTrue();
    }

    [Fact]
    [Trait("Add", "UserScoping")]
    public async Task Add_Should_OverwriteProvidedForeignUserIdWithCurrentUser()
    {
        var repo = Resolve<IFixedTaskRepository>();

        // The caller claims the row belongs to another user; SaveChanges must stamp the current user.
        var task = new FixedTask
        {
            UserId = OtherUsers.First().UserId,
            Name = "Test Task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)
        };

        var created = await repo.AddAndSaveAsync(task, TestContext.Current.CancellationToken);

        created.UserId.Should().Be(CurrentUser.UserId);

        var persisted = await Db.Set<FixedTask>().SingleAsync(t => t.Id == created.Id, TestContext.Current.CancellationToken);
        persisted.UserId.Should().Be(CurrentUser.UserId);
    }

    [Fact]
    [Trait("Update", "UserScoping")]
    public async Task Update_Should_RejectAnotherUsersEntity()
    {
        var otherUsersCategory = await SeedCategoryForOtherUser();
        var repo = Resolve<ICategoryRepository>();

        otherUsersCategory.Description = "Updated Description";
        var act = async () => await repo.UpdateAndSaveAsync(otherUsersCategory, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    [Trait("Update", "UserScoping")]
    public async Task Update_Should_RejectChangingUserIdToAnotherUser()
    {
        var category = await Resolve<SeedDataBuilder<ICategoryRepository, Category, Guid>>().SeedForCurrentUser();
        var repo = Resolve<ICategoryRepository>();

        // Reassigning an owned row to another user violates the RLS WITH CHECK policy.
        category.UserId = OtherUsers.First().UserId;
        var act = async () => await repo.UpdateAndSaveAsync(category, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<DbUpdateException>();

        // The row must still belong to the current user.
        var persisted = await Db.Set<Category>().SingleAsync(c => c.Id == category.Id, TestContext.Current.CancellationToken);
        persisted.UserId.Should().Be(CurrentUser.UserId);
    }

    [Fact]
    [Trait("Delete", "UserScoping")]
    public async Task Delete_Should_RejectAnotherUsersEntity()
    {
        var otherUsersCategory = await SeedCategoryForOtherUser();
        var repo = Resolve<ICategoryRepository>();

        var act = async () => await repo.DeleteAndSaveAsync(otherUsersCategory, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<NotFoundException>();

        // The row must still be present in the table.
        var stillExists = await Db.Set<Category>().AnyAsync(c => c.Id == otherUsersCategory.Id, TestContext.Current.CancellationToken);
        stillExists.Should().BeTrue();
    }

    [Fact]
    [Trait("Add", "UserScoping")]
    public async Task Add_Should_OverwriteForeignUserIdOnNavigationChild()
    {
        var otherUserId = OtherUsers.First().UserId;

        // A task owned by the current user, but whose nested ScheduleEntity (incorrectly) claims another
        // user. ScheduleEntity.UserId is not part of any FK to the task, so only the repository's
        // SaveChanges stamping can correct it - EF relationship fixup won't.
        var task = new FixedTask
        {
            Name = "Task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
            ScheduleEntity = new ScheduleEntity { UserId = otherUserId, RepeatingEntity = GraphSeeder.DailyRepeat() }
        };

        await Resolve<IFixedTaskRepository>().AddAndSaveAsync(task, TestContext.Current.CancellationToken);

        Db.ChangeTracker.Clear();
        var persistedScheduleEntity = await Db.Set<ScheduleEntity>().SingleAsync(TestContext.Current.CancellationToken);
        persistedScheduleEntity.UserId.Should().Be(CurrentUser.UserId);
    }

    [Fact]
    [Trait("Update", "UserScoping")]
    public async Task Update_Should_RejectForeignUserIdOnNavigationChild()
    {
        // A schedule entity that genuinely belongs to another user.
        var foreignSchedule = await SeedScheduleForOtherUser();

        var dbContext = Resolve<TimeHackerDbContext>();
        var scheduleRepo = Resolve<IScheduleEntityRepository>();

        // A task graph for the current user whose schedule child is a stub pointing at another user's
        // row, marked Modified. The foreign row is invisible under RLS, so the UPDATE affects 0 rows
        // and is mapped to NotFound for the current user.
        var task = BuildTaskWithForeignScheduleStub(foreignSchedule.Id);
        dbContext.Attach(task);
        dbContext.Entry(task.ScheduleEntity!).State = EntityState.Modified;

        var act = async () => await scheduleRepo.SaveChangesAsync(TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    [Trait("Delete", "UserScoping")]
    public async Task Delete_Should_RejectForeignUserIdOnNavigationChild()
    {
        var foreignSchedule = await SeedScheduleForOtherUser();

        var dbContext = Resolve<TimeHackerDbContext>();
        var scheduleRepo = Resolve<IScheduleEntityRepository>();

        // Same stub graph, but the foreign schedule child is marked Deleted. Deleting a row owned by
        // another user hits 0 rows under RLS -> NotFound.
        var task = BuildTaskWithForeignScheduleStub(foreignSchedule.Id);
        dbContext.Attach(task);
        dbContext.Entry(task.ScheduleEntity!).State = EntityState.Deleted;

        var act = async () => await scheduleRepo.SaveChangesAsync(TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<NotFoundException>();

        // The other user's row must still be present.
        var stillExists = await Db.Set<ScheduleEntity>().AnyAsync(s => s.Id == foreignSchedule.Id, TestContext.Current.CancellationToken);
        stillExists.Should().BeTrue();
    }

    private FixedTask BuildTaskWithForeignScheduleStub(Guid foreignScheduleId) => new()
    {
        UserId = CurrentUser.UserId,
        Name = "Task",
        Priority = 1,
        StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
        EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
        ScheduleEntity = new ScheduleEntity
        {
            Id = foreignScheduleId,
            UserId = CurrentUser.UserId,
            RepeatingEntity = GraphSeeder.DailyRepeat()
        }
    };

    private Task<Category> SeedCategoryForOtherUser()
        => OtherUsers.First().Resolve<SeedDataBuilder<ICategoryRepository, Category, Guid>>().SeedForCurrentUser();

    private Task<ScheduleEntity> SeedScheduleForOtherUser()
        => OtherUsers.First().Resolve<SeedDataBuilder<IScheduleEntityRepository, ScheduleEntity, Guid>>()
            .SeedForCurrentUser();
}
