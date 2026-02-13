namespace TimeHacker.Application.Api.Tests.AppServiceTests.Users;

public class UserServiceTest
{
    #region Mocks

    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    #endregion

    #region Properties & constructor

    private List<User> _users = null!;

    private readonly IUserAppService _userService;
    private readonly Guid _userId = Guid.NewGuid();

    public UserServiceTest()
    {
        var userAccessor = new UserAccessorBaseMock(_userId, true);
        SetupMocks();
        _userService = new UserService(_userRepositoryMock.Object, userAccessor);
    }

    #endregion

    [Fact]
    [Trait("GetCurrent", "Should return current user")]
    public async Task GetCurrent_ShouldReturnCurrentUser()
    {
        var expectedUser = new User
        {
            Id = _userId,
            Name = "Current User",
            EmailForNotifications = "current@test.com",
            PhoneNumberForNotifications = "+37125844165"
        };
        _users.Add(expectedUser);

        var result = await _userService.GetCurrent(TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Id.Should().Be(_userId);
        result.Name.Should().Be("Current User");
        result.EmailForNotifications.Should().Be("current@test.com");
    }

    [Fact]
    [Trait("GetCurrent", "Should return null for non-existent user")]
    public async Task GetCurrent_ShouldReturnNullForNonExistentUser()
    {

        var result = await _userService.GetCurrent(TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    [Trait("UpdateAsync", "Should update current user")]
    public async Task UpdateAsync_ShouldUpdateCurrentUser()
    {
        var existingUser = new User
        {
            Id = _userId,
            Name = "Old Name",
            EmailForNotifications = "old@test.com"
        };
        _users.Add(existingUser);

        var updateDto = new UserDto
        {
            Id = _userId,
            Name = "Updated Name",
            EmailForNotifications = "updated@test.com",
            PhoneNumberForNotifications = "+37125844166"
        };

        await _userService.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        var updatedUser = _users.First(x => x.Id == _userId);
        updatedUser.Name.Should().Be("Updated Name");
        updatedUser.EmailForNotifications.Should().Be("updated@test.com");
        updatedUser.PhoneNumberForNotifications.Should().Be("+37125844166");
    }

    [Fact]
    [Trait("UpdateAsync", "Should throw when user does not exist")]
    public async Task UpdateAsync_ShouldThrowUserDoesNotExistException()
    {
        var updateDto = new UserDto
        {
            Id = _userId,
            Name = "Updated Name"
        };

        await Assert.ThrowsAsync<UserDoesNotExistException>(() =>
            _userService.UpdateAsync(updateDto, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("UpdateAsync", "Should not update other users")]
    public async Task UpdateAsync_ShouldNotUpdateOtherUsers()
    {
        var currentUser = new User
        {
            Id = _userId,
            Name = "Current User",
            EmailForNotifications = "current@test.com"
        };
        var otherUserId = Guid.NewGuid();
        var otherUser = new User
        {
            Id = otherUserId,
            Name = "Other User",
            EmailForNotifications = "other@test.com"
        };
        _users.Add(currentUser);
        _users.Add(otherUser);

        var updateDto = new UserDto
        {
            Id = _userId,
            Name = "Updated Current User"
        };

        await _userService.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        var updatedCurrentUser = _users.First(x => x.Id == _userId);
        updatedCurrentUser.Name.Should().Be("Updated Current User");

        var unchangedOtherUser = _users.First(x => x.Id == otherUserId);
        unchangedOtherUser.Name.Should().Be("Other User");
        unchangedOtherUser.EmailForNotifications.Should().Be("other@test.com");
    }

    [Fact]
    [Trait("DeleteAsync", "Should delete current user")]
    public async Task DeleteAsync_ShouldDeleteCurrentUser()
    {
        var userToDelete = new User
        {
            Id = _userId,
            Name = "User To Delete"
        };
        _users.Add(userToDelete);

        await _userService.DeleteAsync(TestContext.Current.CancellationToken);

        _users.Should().NotContain(x => x.Id == _userId);
    }

    [Fact]
    [Trait("DeleteAsync", "Should throw when unauthorized")]
    public async Task DeleteAsync_ShouldThrowWhenUnauthorized()
    {
        var unauthorizedAccessor = new UserAccessorBaseMock(_userId, false);
        var unauthorizedService = new UserService(_userRepositoryMock.Object, unauthorizedAccessor);

        await Assert.ThrowsAnyAsync<Exception>(() =>
            unauthorizedService.DeleteAsync(TestContext.Current.CancellationToken));
    }

    #region Mock helpers

    private void SetupMocks()
    {
        _users =
        [
            new()
            {
                Id = Guid.NewGuid(),
                Name = "TestCategory1",
                EmailForNotifications = "test@test.com",
                PhoneNumberForNotifications = "+37125844165"
            },

            new()
            {
                Id  = Guid.NewGuid(),
                Name = "TestName",
                EmailForNotifications = "test@test.com",
            },

            new()
            {
                Id = Guid.NewGuid(),
                Name = "TestName"
            }
        ];

        _userRepositoryMock.As<IRepositoryBase<User, Guid>>().SetupRepositoryMock(_users);
    }

    #endregion
}
