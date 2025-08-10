using AwesomeAssertions;
using Moq;
using TimeHacker.Application.Api.AppServices.Users;
using TimeHacker.Application.Api.Contracts.DTOs.Users;
using TimeHacker.Application.Api.Contracts.IAppServices.Users;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.IRepositories.Users;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Helpers.Tests.Mocks;
using TimeHacker.Helpers.Tests.Mocks.Extensions;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.Users
{
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
}
