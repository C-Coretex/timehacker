using TimeHacker.Domain.Contracts.IModels;

namespace TimeHacker.Domain.Tests.Mocks
{
    internal class UserAccessorMock: IUserAccessor
    {
        public UserAccessorMock(string userId, bool isUserValid)
        {
            UserId = userId;
            IsUserValid = isUserValid;
        }
    }
}
