using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeHacker.Tests.Helpers
{
    internal static class SignInManagerMocker
    {
        public static Mock<UserManager<TUser>> GetUserManagerMock<TUser>() where TUser : class
        {
            return new Mock<UserManager<TUser>>(
            Mock.Of<IUserStore<TUser>>(),
            null,
            Mock.Of<IPasswordHasher<TUser>>(),
            null,
            null,
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            null,
            Mock.Of<ILogger<UserManager<TUser>>>());
        }

        public static Mock<SignInManager<TUser>> GetSignInManagerMock<TUser>(Mock<UserManager<TUser>> userManagerMock) where TUser : class
        {
            return new (
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<TUser>>(),
                null,
                Mock.Of<ILogger<SignInManager<TUser>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<TUser>>());
        }

        public static (Mock<UserManager<TUser>> UserManagerMock, Mock<SignInManager<TUser>> SignInManagerMock) GetIdentityMocks<TUser>() where TUser : class
        {
            var userManagerMock = GetUserManagerMock<TUser>();
            var signInManagerMock = GetSignInManagerMock(userManagerMock);

            return (userManagerMock, signInManagerMock);
        }
    }
}
