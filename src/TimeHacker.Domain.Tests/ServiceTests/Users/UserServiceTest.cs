using AwesomeAssertions;
using Moq;
using TimeHacker.Domain.Models.InputModels.Users;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.IRepositories.Users;
using TimeHacker.Domain.IServices.Users;
using TimeHacker.Domain.Services.Services.Users;
using TimeHacker.Domain.Tests.Mocks;
using TimeHacker.Domain.Tests.Mocks.Extensions;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Tests.ServiceTests.Users
{
    public class UserServiceTest
    {
        #region Mocks

        private readonly Mock<IUserRepository> _userRepositoryMock = new();

        #endregion

        #region Properties & constructor

        private List<User> _users;

        private readonly IUserService _userService;
        private readonly Guid _userId = Guid.NewGuid();

        public UserServiceTest()
        {
            var userAccessor = new UserAccessorBaseMock(_userId, true);
            SetupMocks();
            _userService = new UserService(_userRepositoryMock.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct Id")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var newEntry = new UserUpdateModel()
            {
                Name = "TestCategory1000",
                EmailForNotifications = "aaa",
                PhoneNumberForNotifications = "222"
            };
            await _userService.AddAsync(newEntry);
            var result = _users.FirstOrDefault(x => x.Id == _userId);

            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
            result!.EmailForNotifications.Should().Be(newEntry.EmailForNotifications);
            result!.PhoneNumberForNotifications.Should().Be(newEntry.PhoneNumberForNotifications);
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
}
