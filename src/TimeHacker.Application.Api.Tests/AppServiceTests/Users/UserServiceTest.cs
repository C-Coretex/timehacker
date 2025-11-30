namespace TimeHacker.Application.Api.Tests.AppServiceTests.Users;

public class UserServiceTest
{
    #region Mocks

    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    #endregion

    #region Properties & constructor

    private List<User> _users;

    private readonly IUserAppService _userService;
    private readonly Guid _userId = Guid.NewGuid();

    public UserServiceTest()
    {
        var userAccessor = new UserAccessorBaseMock(_userId, true);
        SetupMocks();
        _userService = new UserService(_userRepositoryMock.Object, userAccessor);
    }

    #endregion


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
