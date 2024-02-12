using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.BusinessLogic.Services;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Tests
{
    public class TasksServiceTests
    {
        #region Mocks
        private readonly Mock<IDynamicTasksServiceQuery> _dynamicTasksServiceQuery = new();
        private readonly Mock<IFixedTasksServiceQuery> _fixedTasksServiceQuery = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
        #endregion

        TasksService _tasksService;
        public TasksServiceTests()
        {
            _httpContextAccessor.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, "TestUser"),
                new(ClaimTypes.NameIdentifier, "TestIdentifier")
            })));

            _tasksService = new TasksService(_dynamicTasksServiceQuery.Object, _fixedTasksServiceQuery.Object, _httpContextAccessor.Object);
        }



        [Fact]
        public async Task GetTasksForDay_ShouldReturnTasksForDay()
        {
            // Arrange
            var date = DateTime.Now;
            var userId = "TestIdentifier";
            _httpContextAccessor.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, "TestUser"),
                new(ClaimTypes.NameIdentifier, userId)
            })));

            var dynamicTasks = new List<DynamicTask>
            {
                new() { 
                    Id = 1, 
                    UserId = userId, 
                    Name = "TestDynamicTask1",
                    Priority = 1,
                    Category = "Test Category", 
                    Description = "Test description",
                    MinTimeToFinish = new TimeSpan(0, 30, 0),
                    MaxTimeToFinish = new TimeSpan(1, 0, 0),
                    OptimalTimeToFinish = new TimeSpan(0, 45, 0),
                },
                new() {
                    Id = 2,
                    UserId = userId,
                    Name = "TestDynamicTask2",
                    Priority = 1,
                    Category = "Test Category",
                    Description = "Test description",
                    MinTimeToFinish = new TimeSpan(0, 30, 0),
                    MaxTimeToFinish = new TimeSpan(1, 0, 0),
                    OptimalTimeToFinish = new TimeSpan(0, 45, 0),
                },
                new() {
                    Id = 3,
                    UserId = "IncorrectUserId",
                    Name = "TestDynamicTask3",
                    Priority = 1,
                    Category = "Test Category",
                    Description = "Test description",
                    MinTimeToFinish = new TimeSpan(0, 30, 0),
                    MaxTimeToFinish = new TimeSpan(1, 0, 0),
                    OptimalTimeToFinish = new TimeSpan(0, 45, 0),
                },
            };
            var mockDynamicTasks = dynamicTasks.AsQueryable().BuildMock();
            _dynamicTasksServiceQuery.Setup(x => x.GetAllByUserId(userId)).Returns(mockDynamicTasks.Where(x => x.UserId == userId));

            var fixedTasks = new List<FixedTask>
            {
                new() { 
                    Id = 1, 
                    UserId = userId, 
                    Name = "TestFixedTask1",
                    Priority = 1,
                    Category = "Test Category",
                    Description = "Test description",
                    StartTimestamp = date.AddHours(1), 
                    EndTimestamp = date.AddHours(1).AddMinutes(30),
                },
                new()
                {
                    Id = 2,
                    UserId = userId,
                    Name = "TestFixedTask2",
                    Priority = 1,
                    Category = "Test Category",
                    Description = "Test description",
                    StartTimestamp = date.AddHours(2),
                    EndTimestamp = date.AddHours(2).AddMinutes(30),
                },
                new()
                {
                    Id = 3,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask3",
                    Priority = 1,
                    Category = "Test Category",
                    Description = "Test description",
                    StartTimestamp = date.AddHours(3),
                    EndTimestamp = date.AddHours(3).AddMinutes(30),
                },
                new()
                {
                    Id = 3,
                    UserId = userId,
                    Name = "TestFixedTask4",
                    Priority = 1,
                    Category = "Test Category",
                    Description = "Test description",
                    StartTimestamp = date.AddDays(-2).AddHours(3),
                    EndTimestamp = date.AddHours(3).AddMinutes(30),
                },
            };
            var mockFixedTasks = fixedTasks.AsQueryable().BuildMock();
            _fixedTasksServiceQuery.Setup(x => x.GetAllByUserId(userId)).Returns(mockFixedTasks.Where(x => x.UserId == userId));

            // Act
            var result = await _tasksService.GetTasksForDay(date.Date);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask1"));
            Assert.True(result.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask2"));
            Assert.False(result.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask3"));
            Assert.False(result.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask4"));
        }
    }
}
