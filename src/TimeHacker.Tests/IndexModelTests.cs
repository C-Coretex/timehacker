using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Application.Models.PageModels;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.BusinessLogic.Services;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Pages;
using TimeHacker.Tests.Helpers;

namespace TimeHacker.Tests
{
    public class IndexModelTests
    {
        private readonly Mock<ILogger<IndexModel>> _logger = new();
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
        private readonly Mock<IDynamicTasksServiceCommand> _dynamicTasksServiceCommand = new();
        private readonly Mock<IFixedTasksServiceCommand> _fixedTasksServiceCommand = new();
        private IndexModel _indexModel
        {
            get
            {
                return new IndexModel(
                    _logger.Object,
                    _signInManagerMock.Object,
                    _httpContextAccessor.Object,
                    _dynamicTasksServiceCommand.Object,
                    _fixedTasksServiceCommand.Object);
            }
        }
        public IndexModelTests()
        {
            (_userManagerMock, _signInManagerMock) = SignInManagerMocker.GetIdentityMocks<IdentityUser>();

            _signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(true);
        }

        [Fact]
        public async Task OnPostDynamicTaskFormHandler_ShouldAddDynamicTask_WhenModelStateIsValid()
        {
            // Arrange
            _httpContextAccessor.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, "TestIdentifier")
            })));

            var indexModel = _indexModel;

            var dynamicTaskList = new List<DynamicTask>();

            _dynamicTasksServiceCommand.Setup(x => x.AddAsync(It.IsAny<DynamicTask>(), It.IsAny<bool>()))
                .Callback<DynamicTask, bool>((dynamicTask, saveChanges) =>
                {
                    if(saveChanges)
                        dynamicTaskList.Add(dynamicTask);
                })
                .Returns< DynamicTask, bool>((dynamicTask, saveChanges) =>  Task.FromResult(dynamicTask));

            var inputDynamicTaskModel = new InputDynamicTaskModel
            {
                Name = "Test Name",
                Description = "Test Description",
                Category = "Test Category",
                MaxTimeToFinish = new (2, 0, 0),
                MinTimeToFinish = new(0, 30, 0),
                OptimalTimeToFinish = new(1, 25, 0),
                Priority = 2
            };

            // Act
            var result = await indexModel.OnPostDynamicTaskFormHandler(inputDynamicTaskModel);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            //check if element is in the list
            Assert.Contains(dynamicTaskList, x => x.Name == inputDynamicTaskModel.Name && x.Category == inputDynamicTaskModel.Category);
        }

        [Fact]
        public async Task OnPostDynamicTaskFormHandler_ShouldReturnException_IfUserIdIsEmpty()
        {
            // Arrange
            _httpContextAccessor.Setup(x => x.HttpContext.User).Returns((ClaimsPrincipal)null);

            var indexModel = _indexModel;

            var inputDynamicTaskModel = new InputDynamicTaskModel
            {
                Name = "Test Name",
                Description = "Test Description",
                Category = "Test Category",
                MaxTimeToFinish = new(2, 0, 0),
                MinTimeToFinish = new(0, 30, 0),
                OptimalTimeToFinish = new(1, 25, 0),
                Priority = 2
            };

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await indexModel.OnPostDynamicTaskFormHandler(inputDynamicTaskModel));

            // Assert
        }
    }
}
