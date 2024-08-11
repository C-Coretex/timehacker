using TimeHacker.Domain.Contracts.IModels;

namespace TimeHacker.Tests.Mocks
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
