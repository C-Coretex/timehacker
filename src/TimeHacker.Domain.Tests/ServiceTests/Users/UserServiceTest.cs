using FluentAssertions;
using Moq;
using TimeHacker.Domain.Contracts.Entities.Users;
using TimeHacker.Domain.Contracts.IRepositories.Users;
using TimeHacker.Domain.Contracts.IServices.Users;
using TimeHacker.Domain.Contracts.Models.InputModels.Users;
using TimeHacker.Domain.Services.Users;
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

        public UserServiceTest()
        {
            var userAccessor = new UserAccessorBaseMock("TestIdentifier", true);

            _userService = new UserService(_userRepositoryMock.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct Id")]
        public async Task AddAsync_ShouldAddEntry()
        {
            SetupMocks("UniqueId");

            var newEntry = new UserUpdateModel()
            {
                Name = "TestCategory1000",
                EmailForNotifications = "aaa",
                PhoneNumberForNotifications = "222",
            };
            await _userService.AddAsync(newEntry);
            var result = _users.FirstOrDefault(x => x.Id == "TestIdentifier");
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
            result!.EmailForNotifications.Should().Be(newEntry.EmailForNotifications);
            result!.PhoneNumberForNotifications.Should().Be(newEntry.PhoneNumberForNotifications);
        }

        #region Mock helpers

        private void SetupMocks(string userId)
        {
            _users =
            [
                new()
                {
                    Id = userId,
                    Name = "TestCategory1",
                    EmailForNotifications = "test@test.com",
                    PhoneNumberForNotifications = "+37125844165"
                },

                new()
                {
                    Id = "IncorrectUserId",
                    Name = "TestName",
                    EmailForNotifications = "test@test.com",
                },

                new()
                {
                    Id = "IncorrectUserId",
                    Name = "TestName"
                }
            ];

            _userRepositoryMock.As<IRepositoryBase<User, string>>().SetupRepositoryMock(_users);
        }

        #endregion
    }
}
