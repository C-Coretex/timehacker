﻿using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Services.Tasks;

namespace TimeHacker.Tests
{/*
    public class TasksServiceTests
    {
        #region Mocks

        private readonly Mock<IDynamicTaskService> _dynamicUserTasksServiceQuery = new();
        private readonly Mock<IDynamicTaskService> _dynamicUserTasksServiceCommand = new();
        private readonly Mock<IFixedTaskService> _fixedUserTasksServiceQuery = new();
        private readonly Mock<IFixedTaskService> _fixedUserTasksServiceCommand = new();
        private readonly Mock<IUserAccessor> _userAccessor = new();

        #endregion

        ITaskService _tasksService;
        public TasksServiceTests()
        {
            _userAccessor.Setup(x => x.UserId).Returns("TestIdentifier");
            _userAccessor.Setup(x => x.IsUserValid).Returns(true);

            var dynamicUserTasksService = new DynamicTaskService(_dynamicUserTasksServiceCommand.Object, _dynamicUserTasksServiceQuery.Object);
            var fixedUserTasksService = new FixedTaskService(_fixedUserTasksServiceCommand.Object, _fixedUserTasksServiceQuery.Object);
            _tasksService = new TaskService(dynamicUserTasksService, fixedUserTasksService);
        }

        [Fact]
        public async Task GetTasksForDay_ShouldReturnTasksForDay()
        {
            // Arrange
            var date = DateTime.Now;
            var userId = "TestIdentifier";

            var dynamicTasks = new List<DynamicTask>
            {
                new() { 
                    Id = 1, 
                    UserId = userId, 
                    Name = "TestDynamicTask1",
                    Priority = 1,
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
                    Description = "Test description",
                    MinTimeToFinish = new TimeSpan(0, 30, 0),
                    MaxTimeToFinish = new TimeSpan(1, 0, 0),
                    OptimalTimeToFinish = new TimeSpan(0, 45, 0),
                },
            };
            var mockDynamicTasks = dynamicTasks.AsQueryable().BuildMock();
            _dynamicUserTasksServiceQuery.Setup(x => x.GetAllByUserId(userId)).Returns(mockDynamicTasks.Where(x => x.UserId == userId));

            var fixedTasks = new List<FixedTask>
            {
                new() { 
                    Id = 1, 
                    UserId = userId, 
                    Name = "TestFixedTask1",
                    Priority = 1,
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
                    Description = "Test description",
                    StartTimestamp = date.AddDays(-2).AddHours(3),
                    EndTimestamp = date.AddHours(3).AddMinutes(30),
                },
            };
            var mockFixedTasks = fixedTasks.AsQueryable().BuildMock();
            _fixedUserTasksServiceQuery.Setup(x => x.GetAllByUserId(userId)).Returns(mockFixedTasks.Where(x => x.UserId == userId));

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
*/}
