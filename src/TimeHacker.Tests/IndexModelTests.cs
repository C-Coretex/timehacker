using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TimeHacker.Application.Models.PageModels;
using TimeHacker.Application.Pages;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Tests.Helpers;

namespace TimeHacker.Tests
{
    public class IndexModelTests
    {
        #region Mocks
        private readonly Mock<ILogger<IndexModel>> _logger = new();
        private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
        private readonly Mock<IDynamicTasksServiceCommand> _dynamicTasksServiceCommand = new();
        private readonly Mock<IFixedTasksServiceCommand> _fixedTasksServiceCommand = new();
        #endregion

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
            (_, _signInManagerMock) = SignInManagerMocker.GetIdentityMocks<IdentityUser>();

            _httpContextAccessor.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, "TestUser"),
                new(ClaimTypes.NameIdentifier, "TestIdentifier")
            })));

            _signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(true);
        }

        [Fact]
        public void OnGet_ShouldRedirectToLanding_IfUserIsNotSignedIn()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(false);

            var indexModel = _indexModel;

            // Act
            var result = indexModel.OnGet();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Landing", ((RedirectToPageResult)result).PageName);
        }
        [Fact]
        public void OnGet_ShouldReturnPage_IfUserIsSignedIn()
        {
            // Arrange
            var indexModel = _indexModel;

            // Act
            var result = indexModel.OnGet();

            // Assert
            Assert.IsAssignableFrom<PageResult>(result);
        }

        [Fact]
        public async Task OnPostDynamicTaskFormHandler_ShouldAddDynamicTask_WhenModelStateIsValid()
        {
            // Arrange
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
            var func = async () => await indexModel.OnPostDynamicTaskFormHandler(inputDynamicTaskModel);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(func);
        }
        [Fact]
        public async Task OnPostDynamicTaskFormHandler_ShouldReturnException_IfModelStateIsInvalid()
        {
            // Arrange
            var indexModel = _indexModel;

            var inputDynamicTaskModel = new InputDynamicTaskModel
            {
                Name = null,
                Description = "Test Description",
                Category = "Test Category",
                MaxTimeToFinish = new(2, 0, 0),
                MinTimeToFinish = new(0, 30, 0),
                OptimalTimeToFinish = new(1, 25, 0),
                Priority = 2
            };

            // Act
            var result = await indexModel.OnPostDynamicTaskFormHandler(inputDynamicTaskModel);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(indexModel.ModelState.IsValid);
        }

        [Fact]
        public async Task OnPostFixedTaskFormHandler_ShouldAddFixedTask_WhenModelStateIsValid()
        {
            // Arrange
            var indexModel = _indexModel;

            var fixedTaskList = new List<FixedTask>();

            _fixedTasksServiceCommand.Setup(x => x.AddAsync(It.IsAny<FixedTask>(), It.IsAny<bool>()))
                .Callback<FixedTask, bool>((fixedTask, saveChanges) =>
                {
                    if (saveChanges)
                        fixedTaskList.Add(fixedTask);
                })
                .Returns<FixedTask, bool>((fixedTask, saveChanges) => Task.FromResult(fixedTask));

            var inputFixedTaskModel = new InputFixedTaskModel
            {
                Name = "Test Name",
                Description = "Test Description",
                Category = "Test Category",
                EndTimestamp = DateTime.Now.AddHours(-1).ToString("dd-MM-yyyy HH:mm"),
                StartTimestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
                Priority = 2
            };

            // Act
            var result = await indexModel.OnPostFixedTaskFormHandler(inputFixedTaskModel);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            //check if element is in the list
            Assert.Contains(fixedTaskList, x => x.Name == inputFixedTaskModel.Name && x.Category == inputFixedTaskModel.Category);
        }
        [Fact]
        public async Task OnPostFixedTaskFormHandler_ShouldReturnException_IfUserIdIsEmpty()
        {
            // Arrange
            _httpContextAccessor.Setup(x => x.HttpContext.User).Returns((ClaimsPrincipal)null);

            var indexModel = _indexModel;

            var inputFixedTaskModel = new InputFixedTaskModel
            {
                Name = "Test Name",
                Description = "Test Description",
                Category = "Test Category",
                EndTimestamp = DateTime.Now.AddHours(-1).ToString("dd-MM-yyyy HH:mm"),
                StartTimestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
                Priority = 2
            };

            // Act
            var func = async () => await indexModel.OnPostFixedTaskFormHandler(inputFixedTaskModel);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(func);
        }
        [Fact]
        public async Task OnPostFixedTaskFormHandler_ShouldReturnException_IfModelStateIsInvalid()
        {
            // Arrange
            var indexModel = _indexModel;

            var inputFixedTaskModel = new InputFixedTaskModel
            {
                Name = null,
                Description = "Test Description",
                Category = "Test Category",
                EndTimestamp = DateTime.Now.AddHours(-1).ToString("dd-MM-yyyy HH:mm"),
                StartTimestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
                Priority = 2
            };

            // Act
            var result = await indexModel.OnPostFixedTaskFormHandler(inputFixedTaskModel);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(indexModel.ModelState.IsValid);
        }
    }
}
