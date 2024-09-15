using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.EntityModels;
using TimeHacker.Domain.Contracts.Models.EntityModels.Enums;
using TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Services.Categories;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Domain.Services.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Tests.Helpers;
using TimeHacker.Tests.Mocks;

namespace TimeHacker.Tests.ServiceTests.Tasks;

public class TaskServiceTests
{
    #region Mocks

    private readonly Mock<IDynamicTaskRepository> _dynamicUserTasksRepository = new();
    private readonly Mock<IFixedTaskRepository> _fixedUserTasksRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IScheduleSnapshotRepository> _scheduleSnapshotRepository = new();

    private readonly Mock<IScheduleEntityRepository> _scheduleEntityRepository = new();

    private readonly IUserAccessor _userAccessor;

    #endregion

    private List<FixedTask> _fixedTasks;
    private List<DynamicTask> _dynamicTasks;
    private List<ScheduleSnapshot> _scheduleSnapshots;
    private List<ScheduleEntity> _scheduleEntities;

    private readonly ITaskService _tasksService;

    public TaskServiceTests()
    {
        _userAccessor = new UserAccessorMock("TestIdentifier", true);
        var mapperConfiguration = AutomapperHelpers.GetMapperConfiguration();
        var mapper = new Mapper(mapperConfiguration);

        var dynamicTasksService = new DynamicTaskService(_dynamicUserTasksRepository.Object, _userAccessor);
        var fixedTasksService = new FixedTaskService(_fixedUserTasksRepository.Object, _userAccessor);
        var scheduleSnapshotService = new ScheduleSnapshotService(_scheduleSnapshotRepository.Object, _userAccessor);
        var categoryService = new CategoryService(_categoryRepository.Object, _userAccessor);
        var scheduleEntityService = new ScheduleEntityService(_scheduleEntityRepository.Object, fixedTasksService,
            categoryService, _userAccessor, mapper);

        _tasksService = new TaskService(fixedTasksService, dynamicTasksService, scheduleSnapshotService,
            scheduleEntityService, mapper);
    }

    [Fact]
    public async Task GetTasksForDay_ShouldReturnTasksForDay()
    {
        // Arrange
        var date = DateTime.Now;
        var userId = "TestIdentifier";
        SetupTaskMocks(date, userId);

        // Act
        var result = await _tasksService.GetTasksForDay(DateOnly.FromDateTime(date));

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
        var dates = new List<DateTime> { DateTime.Now.AddDays(-1), DateTime.Now, DateTime.Now.AddDays(1) };
        var userId = "TestIdentifier";
        SetupTaskMocks(dates[1], userId);

        // Act
        var result = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();

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
        var dates = new List<DateTime> { DateTime.Now.AddDays(-1), DateTime.Now, DateTime.Now.AddDays(1) };
        var userId = "TestIdentifier";
        SetupTaskMocks(dates[1], userId);

        // Act
        var result1 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();
        var result2 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();

        // Assert
        result1.Should().BeEquivalentTo(result2, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
    }

    [Fact]
    public async Task GetTasksForDays_ShouldRefreshSnapshot()
    {
        // Arrange
        var dates = new List<DateTime> { DateTime.Now.AddDays(-1), DateTime.Now, DateTime.Now.AddDays(1) };
        var userId = "TestIdentifier";
        SetupTaskMocks(dates[1], userId);

        // Act
        var result1 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();
        var result2 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();
        var result3 = await _tasksService.RefreshTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();
        var result4 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();

        // Assert
        result1.Should().BeEquivalentTo(result2, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
        result2.Should().NotBeEquivalentTo(result3, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
        result3.Should().BeEquivalentTo(result4, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
    }

    [Fact]
    public async Task GetTasksForDays_ShouldBeEmptyForUserWithoutTasks()
    {
        // Arrange
        var dates = new List<DateTime> { DateTime.Now.AddDays(-1), DateTime.Now, DateTime.Now.AddDays(1) };
        var userId = "IncorrectIdentifier";
        SetupTaskMocks(dates[1], userId);

        // Act
        var result = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(dates.Count);
        result.Should().OnlyContain(x => x.TasksTimeline.Count == 0);
    }

    [Fact]
    public async Task GetTasksForDays_ShouldAddScheduledTasks()
    {
        // Arrange
        var dates = new List<DateTime> { DateTime.Now.AddDays(-1), DateTime.Now, DateTime.Now.AddDays(1) };
        var userId = "TestIdentifier";
        SetupTaskMocks(dates[1], userId);
        var fixedTask = new FixedTask()
        {
            Id = 100,
            UserId = userId,
            Name = "TestFixedTask1",
            Priority = 1,
            Description = "Test description",
            StartTimestamp = dates[0].AddHours(1),
            EndTimestamp = dates[0].AddHours(1).AddMinutes(30),
            ScheduleEntityId = 1
        };
        _fixedTasks.Add(fixedTask);

        var scheduleEntities = new List<ScheduleEntity>()
        {
            new()
            {
                Id = 1,
                UserId = userId,
                LastEntityCreated = null,
                EndsOn = null,
                CreatedTimestamp = dates[0],
                RepeatingEntity = new RepeatingEntityModel()
                {
                    EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                    RepeatingData = new DayRepeatingEntity(1)
                },
                FixedTask = fixedTask
            }
        };

        _scheduleEntityRepository
            .Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<ScheduleEntity>[]>()))
            .Returns(scheduleEntities.AsQueryable().BuildMock());
        

        // Act
        var result1 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();
        var result2 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime)).ToListAsync();

        // Assert
        result1.Should().NotBeNull();
        result1.Should().HaveCount(dates.Count);
        result1.Should().Contain(x => x.TasksTimeline.Any(y => y.Task.Id == fixedTask.Id));

        result1.Should().BeEquivalentTo(result2, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
    }


    #region Mock helpers

    private void SetupTaskMocks(DateTime date, string userId)
    {
        _dynamicTasks =
        [
            new()
            {
                Id = 1,
                UserId = userId,
                Name = "TestDynamicTask1",
                Priority = 1,
                Description = "Test description",
                MinTimeToFinish = new TimeSpan(0, 30, 0),
                MaxTimeToFinish = new TimeSpan(1, 0, 0),
                OptimalTimeToFinish = new TimeSpan(0, 45, 0)
            },

            new()
            {
                Id = 2,
                UserId = userId,
                Name = "TestDynamicTask2",
                Priority = 1,
                Description = "Test description",
                MinTimeToFinish = new TimeSpan(0, 30, 0),
                MaxTimeToFinish = new TimeSpan(1, 0, 0),
                OptimalTimeToFinish = new TimeSpan(0, 45, 0)
            },

            new()
            {
                Id = 3,
                UserId = "IncorrectUserId",
                Name = "TestDynamicTask3",
                Priority = 1,
                Description = "Test description",
                MinTimeToFinish = new TimeSpan(0, 30, 0),
                MaxTimeToFinish = new TimeSpan(1, 0, 0),
                OptimalTimeToFinish = new TimeSpan(0, 45, 0)
            }
        ];
        _dynamicUserTasksRepository.Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<DynamicTask>[]>()))
            .Returns(_dynamicTasks.AsQueryable().BuildMock());

        _fixedTasks =
        [
            new()
            {
                Id = 1,
                UserId = userId,
                Name = "TestFixedTask1",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = date.AddHours(1),
                EndTimestamp = date.AddHours(1).AddMinutes(30)
            },

            new()
            {
                Id = 2,
                UserId = userId,
                Name = "TestFixedTask2",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = date.AddHours(2),
                EndTimestamp = date.AddHours(2).AddMinutes(30)
            },

            new()
            {
                Id = 3,
                UserId = "IncorrectUserId",
                Name = "TestFixedTask3",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = date.AddHours(3),
                EndTimestamp = date.AddHours(3).AddMinutes(30)
            },

            new()
            {
                Id = 3,
                UserId = "IncorrectUserId",
                Name = "TestFixedTask4",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = date.AddDays(-2).AddHours(3),
                EndTimestamp = date.AddHours(3).AddMinutes(30)
            }
        ];
        _fixedUserTasksRepository.Setup(x => x.GetAll(It.IsAny<bool>()))
            .Returns(_fixedTasks.AsQueryable().BuildMock());

        _scheduleSnapshots = new List<ScheduleSnapshot>();
        _scheduleSnapshotRepository.Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<ScheduleSnapshot>[]>()))
            .Returns(_scheduleSnapshots.AsQueryable().BuildMock());
        _scheduleSnapshotRepository
            .Setup(x => x.GetAll(It.IsAny<bool>(), It.IsAny<IncludeExpansionDelegate<ScheduleSnapshot>[]>()))
            .Returns(_scheduleSnapshots.AsQueryable().BuildMock());
        _scheduleSnapshotRepository.Setup(x => x.AddAsync(It.IsAny<ScheduleSnapshot>(), It.IsAny<bool>()))
            .Callback<ScheduleSnapshot, bool>((model, saveChanges) => { _scheduleSnapshots.Add(model); })
            .Returns<ScheduleSnapshot, bool>((model, saveChanges) => Task.FromResult(model));

        _scheduleSnapshotRepository.Setup(x => x.UpdateAsync(It.IsAny<ScheduleSnapshot>(), It.IsAny<bool>()))
            .Callback<ScheduleSnapshot, bool>((model, saveChanges) =>
            {
                var existingObj =
                    _scheduleSnapshots.First(x => x.UserId == model.UserId && x.Date == model.Date);
                _scheduleSnapshots.Remove(existingObj);
                _scheduleSnapshots.Add(model);
            }).Returns<ScheduleSnapshot, bool>((model, saveChanges) => Task.FromResult(model));

        _scheduleEntities = [];

        _scheduleEntityRepository
            .Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<ScheduleEntity>[]>()))
            .Returns(_scheduleEntities.AsQueryable().BuildMock());
        _scheduleEntityRepository.Setup(x => x.AddAsync(It.IsAny<ScheduleEntity>(), It.IsAny<bool>()))
            .Callback<ScheduleEntity, bool>((model, saveChanges) => { _scheduleEntities.Add(model); })
            .Returns<ScheduleEntity, bool>((model, saveChanges) => Task.FromResult(model));

        _scheduleEntityRepository.Setup(x => x.UpdateAsync(It.IsAny<ScheduleEntity>(), It.IsAny<bool>()))
            .Callback<ScheduleEntity, bool>((model, saveChanges) =>
            {
                var existingObj =
                    _scheduleEntities.First(x => x.Id == model.Id);
                _scheduleEntities.Remove(existingObj);
                _scheduleEntities.Add(model);
            }).Returns<ScheduleEntity, bool>((model, saveChanges) => Task.FromResult(model));
    }

    #endregion
}