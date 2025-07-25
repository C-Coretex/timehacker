using TimeHacker.Domain.IModels;

namespace TimeHacker.Application.Api.Tests.Mocks
{
    internal class UserAccessorBaseMock: UserAccessorBase
    {
        public UserAccessorBaseMock(Guid userId, bool isUserValid)
        {
            UserId = userId;
            IsUserValid = isUserValid;
        }

        public void SetUserId(Guid userId) => UserId = userId;
        public void SetIsValid(bool isValid) => IsUserValid = IsUserValid;
    }
}
