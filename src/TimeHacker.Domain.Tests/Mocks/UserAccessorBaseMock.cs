using TimeHacker.Domain.Contracts.IModels;

namespace TimeHacker.Domain.Tests.Mocks
{
    internal class UserAccessorBaseMock: UserAccessorBase
    {
        public UserAccessorBaseMock(string userId, bool isUserValid)
        {
            UserId = userId;
            IsUserValid = isUserValid;
        }
    }
}
