using Microsoft.EntityFrameworkCore;
using Timehacker.Integration.Db.Tests;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Integration.Db.Tests.Fixtures;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests.UserScopingTests;

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
}
