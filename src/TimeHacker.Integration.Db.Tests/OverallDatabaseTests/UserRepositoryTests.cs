using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.IRepositories.Users;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

// UserRepository is the one repository that is NOT user-scoped (it resolves the domain user itself).
public class UserRepositoryTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("GetAll", "NonScoped")]
    public async Task GetAll_Should_ReturnAllUsers()
    {
        var repo = Resolve<IUserRepository>();

        var result = await repo.GetAll().ToListAsync(TestContext.Current.CancellationToken);

        // Base class seeds the current user + every other user; none are filtered out.
        result.Should().HaveCount(OtherUsers.Count + 1);
        result.Should().Contain(u => u.Id == CurrentUser.UserId);
        foreach (var user in OtherUsers)
            result.Should().Contain(u => u.Id == user.UserId);
    }

    [Fact]
    [Trait("Add", "UniqueIdentityId")]
    public async Task Add_Should_RejectDuplicateIdentityId()
    {
        var repo = Resolve<IUserRepository>();
        var existing = await Db.Set<User>().FirstAsync(TestContext.Current.CancellationToken);

        var duplicate = new User
        {
            Name = "Duplicate identity",
            IdentityId = existing.IdentityId
        };

        var act = async () => await repo.AddAndSaveAsync(duplicate, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<DbUpdateException>();
    }
}
