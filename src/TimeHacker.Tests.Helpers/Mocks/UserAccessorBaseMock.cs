namespace TimeHacker.Helpers.Tests.Mocks
{
    public class UserAccessorBaseMock: UserAccessorBase
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
