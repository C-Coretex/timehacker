using TimeHacker.Domain.IModels;

namespace TimeHacker.Domain.Tests.Mocks
{
    internal class UserAccessorBaseMock: UserAccessorBase
    {
        public UserAccessorBaseMock(Guid userId, bool isUserValid)
        {
            UserId = userId;
            IsUserValid = isUserValid;
        }
    }
}
