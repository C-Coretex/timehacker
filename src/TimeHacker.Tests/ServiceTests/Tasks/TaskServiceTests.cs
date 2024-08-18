using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Domain.Services.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Tests.Helpers;
using TimeHacker.Tests.Mocks;

namespace TimeHacker.Tests.ServiceTests.Tasks
{
    public class TaskServiceTests
    {
        #region Mocks

        private readonly Mock<IDynamicTaskRepository> _dynamicUserTasksRepository = new();
        private readonly Mock<IFixedTaskRepository> _fixedUserTasksRepository = new();

        private readonly Mock<IScheduleSnapshotRepository> _scheduleSnapshotRepository = new();

        private readonly IUserAccessor _userAccessor;

        #endregion

        ITaskService _tasksService;
        public TaskServiceTests()
        {
            _userAccessor = new UserAccessorMock("TestIdentifier", true);

            var dynamicTasksService = new DynamicTaskService(_dynamicUserTasksRepository.Object, _userAccessor);
            var fixedTasksService = new FixedTaskService(_fixedUserTasksRepository.Object, _userAccessor);
            var scheduleSnapshotService = new ScheduleSnapshotService(_scheduleSnapshotRepository.Object, _userAccessor);
            var mapperConfiguration = AutomapperHelpers.GetMapperConfiguration();
            var mapper = new Mapper(mapperConfiguration);
            _tasksService = new TaskService(fixedTasksService, dynamicTasksService, scheduleSnapshotService, mapper);
        }

        [Fact]
        public async Task GetTasksForDay_ShouldReturnTasksForDay()
        {
            // Arrange
            var date = DateTime.Now;
            var userId = "TestIdentifier";
            SetupTaskMocks(date, userId);

            // Act
            var result = await _tasksService.GetTasksForDay(date.Date);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result.TasksTimeline, tt => tt.Task.Name == "TestFixedTask1");
            Assert.Contains(result.TasksTimeline, tt => tt.Task.Name == "TestFixedTask2");
            Assert.DoesNotContain(result.TasksTimeline, tt => tt.Task.Name == "TestFixedTask3");
            Assert.DoesNotContain(result.TasksTimeline, tt => tt.Task.Name == "TestFixedTask4");
        }

        [Fact]
        public async Task GetTasksForDays_ShouldReturnTasksForDays()
        {
            // Arrange
            var dates = new List<DateTime>() { DateTime.Now.AddDays(-1), DateTime.Now, DateTime.Now.AddDays(1) };
            var userId = "TestIdentifier";
            SetupTaskMocks(dates[1], userId);

            // Act
            var result = await _tasksService.GetTasksForDays(dates).ToListAsync();

            // Assert
            Assert.NotNull(result);
            result.Should().HaveCount(dates.Count);
            Assert.Contains(result, x => x.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask1"));
            Assert.Contains(result, x => x.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask2"));
            Assert.DoesNotContain(result, x => x.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask3"));
            Assert.DoesNotContain(result, x => x.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask4"));
        }

        [Fact]
        public async Task GetTasksForDays_ShouldReturnSaveAndReturnSnapshotAfterFirstCall()
        {
            // Arrange
            var dates = new List<DateTime>() { DateTime.Now.AddDays(-1), DateTime.Now, DateTime.Now.AddDays(1) };
            var userId = "TestIdentifier";
            SetupTaskMocks(dates[1], userId);

            // Act
            var result1 = await _tasksService.GetTasksForDays(dates).ToListAsync();
            var result2 = await _tasksService.GetTasksForDays(dates).ToListAsync();

            // Assert
            result1.Should().BeEquivalentTo(result2, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
        }

        [Fact]
        public async Task GetTasksForDays_ShouldRefreshSnapshot()
        {
            // Arrange
            var dates = new List<DateTime>() { DateTime.Now.AddDays(-1), DateTime.Now, DateTime.Now.AddDays(1) };
            var userId = "TestIdentifier";
            SetupTaskMocks(dates[1], userId);

            // Act
            var result1 = await _tasksService.GetTasksForDays(dates).ToListAsync();
            var result2 = await _tasksService.GetTasksForDays(dates).ToListAsync();
            var result3 = await _tasksService.RefreshTasksForDays(dates).ToListAsync();
            var result4 = await _tasksService.GetTasksForDays(dates).ToListAsync();

            // Assert
            result1.Should().BeEquivalentTo(result2, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
            result2.Should().NotBeEquivalentTo(result3, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
            result3.Should().BeEquivalentTo(result4, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
        }

        [Fact]
        public async Task GetTasksForDays_ShouldBeEmptyForUserWithoutTasks()
        {
            // Arrange
            var dates = new List<DateTime>() { DateTime.Now.AddDays(-1), DateTime.Now, DateTime.Now.AddDays(1) };
            var userId = "IncorrectIdentifier";
            SetupTaskMocks(dates[1], userId);

            // Act
            var result = await _tasksService.GetTasksForDays(dates).ToListAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(dates.Count);
            result.Should().OnlyContain(x => x.TasksTimeline.Count == 0);
        }

        #region Mock helpers

        private void SetupTaskMocks(DateTime date, string userId)
        {
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
            _dynamicUserTasksRepository.Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<DynamicTask>[]>())).Returns(dynamicTasks.AsQueryable().BuildMock());

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
            _fixedUserTasksRepository.Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<FixedTask>[]>())).Returns(fixedTasks.AsQueryable().BuildMock());

            var scheduleSnapshots = new List<ScheduleSnapshot>();
            _scheduleSnapshotRepository.Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<ScheduleSnapshot>[]>())).Returns(scheduleSnapshots.AsQueryable().BuildMock());
            _scheduleSnapshotRepository.Setup(x => x.GetAll(It.IsAny<bool>(), It.IsAny<IncludeExpansionDelegate<ScheduleSnapshot>[]>())).Returns(scheduleSnapshots.AsQueryable().BuildMock());
            _scheduleSnapshotRepository.Setup(x => x.AddAsync(It.IsAny<ScheduleSnapshot>(), It.IsAny<bool>())).Callback<ScheduleSnapshot, bool>((model, saveChanges) =>
            {
                scheduleSnapshots.Add(model);
            }).Returns<ScheduleSnapshot, bool>((model, saveChanges) => Task.FromResult(model));

            _scheduleSnapshotRepository.Setup(x => x.UpdateAsync(It.IsAny<ScheduleSnapshot>(), It.IsAny<bool>())).Callback<ScheduleSnapshot, bool>((model, saveChanges) =>
            {
                var existingObj = scheduleSnapshots.FirstOrDefault(x => x.UserId == model.UserId && x.Date == model.Date)!;
                scheduleSnapshots.Remove(existingObj);
                scheduleSnapshots.Add(model);
            }).Returns<ScheduleSnapshot, bool>((model, saveChanges) => Task.FromResult(model));
        }

        #endregion
    }
}
