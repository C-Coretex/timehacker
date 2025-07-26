﻿using AwesomeAssertions;
using MockQueryable;
using Moq;
using TimeHacker.Application.Api.AppServices.Categories;
using TimeHacker.Application.Api.AppServices.ScheduleSnapshots;
using TimeHacker.Application.Api.AppServices.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IProcessors;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.Models.EntityModels;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Services.Processors;
using TimeHacker.Domain.Services.Services;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Tests.Mocks;
using TimeHacker.Helpers.Tests.Mocks.Extensions;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.Tasks;

public class TaskServiceTests
{
    #region Mocks

    private readonly Mock<IDynamicTaskRepository> _dynamicTasksRepository = new();
    private readonly Mock<IFixedTaskRepository> _fixedTasksRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IScheduleSnapshotRepository> _scheduleSnapshotRepository = new();

    private readonly Mock<IScheduleEntityRepository> _scheduleEntityRepository = new();

    #endregion

    #region Properties & constructor

    private List<FixedTask> _fixedTasks;
    private List<DynamicTask> _dynamicTasks;
    private List<ScheduleSnapshot> _scheduleSnapshots;
    private List<ScheduleEntity> _scheduleEntities;

    private readonly ITaskAppService _tasksService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly DateTime _date = DateTime.Now.Date;

    public TaskServiceTests()
    {
        SetupMocks(_date, _userId);

        var taskTimelineProcessor = new TaskTimelineProcessor();
        var scheduleEntityService = new ScheduleEntityService(_scheduleEntityRepository.Object);

        _tasksService = new TaskService(_fixedTasksRepository.Object, _dynamicTasksRepository.Object, _scheduleSnapshotRepository.Object, scheduleEntityService, taskTimelineProcessor);
    }

    #endregion



    [Fact]
    [Trait("GetTasksForDay", "Should return tasks for day")]
    public async Task GetTasksForDay_ShouldReturnTasksForDay()
    {
        var result = await _tasksService.GetTasksForDay(DateOnly.FromDateTime(_date));

        Assert.NotNull(result);
        Assert.Contains(result.TasksTimeline, tt => tt.Task.Name == "TestFixedTask1");
        Assert.Contains(result.TasksTimeline, tt => tt.Task.Name == "TestFixedTask2");
    }

    [Fact]
    [Trait("GetTasksForDays", "Should return tasks for days")]
    public async Task GetTasksForDays_ShouldReturnTasksForDays()
    {
        var dates = new List<DateTime> { DateTime.Now.AddDays(-1), _date, DateTime.Now.AddDays(1) };

        var result = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime).ToList()).ToListAsync();

        Assert.NotNull(result);
        result.Should().HaveCount(dates.Count);
        Assert.Contains(result, x => x.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask1"));
        Assert.Contains(result, x => x.TasksTimeline.Any(tt => tt.Task.Name == "TestFixedTask2"));
    }

    [Fact]
    [Trait("GetTasksForDays", "Should return, save and return snapshot after first call")]
    public async Task GetTasksForDays_ShouldReturnSaveAndReturnSnapshotAfterFirstCall()
    {
        var dates = new List<DateTime> { DateTime.Now.Date.AddDays(-1), _date, DateTime.Now.Date.AddDays(1) };

        var result1 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime).ToList()).ToListAsync();
        var result2 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime).ToList()).ToListAsync();

        result1.Should().BeEquivalentTo(result2, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
    }

    [Fact]
    [Trait("GetTasksForDays", "Should refresh snapshot")]
    public async Task GetTasksForDays_ShouldRefreshSnapshot()
    {
        var dates = new List<DateTime> { DateTime.Now.Date.AddDays(-1), _date, DateTime.Now.Date.AddDays(1) };

        var result1 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime).ToList()).ToListAsync();
        var result2 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime).ToList()).ToListAsync();
        var result3 = await _tasksService.RefreshTasksForDays(dates.Select(DateOnly.FromDateTime).ToList()).ToListAsync();
        var result4 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime).ToList()).ToListAsync();

        result1.Should().BeEquivalentTo(result2, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
        result2.Should().NotBeEquivalentTo(result3, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
        result3.Should().BeEquivalentTo(result4, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
    }

    [Fact]
    [Trait("GetTasksForDays", "Should add scheduled tasks")]
    public async Task GetTasksForDays_ShouldAddScheduledTasks()
    {
        var dates = new List<DateTime> { DateTime.Now.Date.AddDays(-1), _date, DateTime.Now.Date.AddDays(1) };

        var fixedTask = new FixedTask()
        {
            UserId = _userId,
            Name = "TestFixedTask1",
            Priority = 1,
            Description = "Test description",
            StartTimestamp = dates[0].AddHours(1),
            EndTimestamp = dates[0].AddHours(1).AddMinutes(30),
            ScheduleEntityId = Guid.NewGuid()
        };
        _fixedTasks.Add(fixedTask);

        var scheduleEntities = new List<ScheduleEntity>()
        {
            new()
            {
                UserId = _userId,
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
            .Setup(x => x.GetAll(It.IsAny<QueryPipelineStep<ScheduleEntity>[]>()))
            .Returns(scheduleEntities.AsQueryable().BuildMock());
        

        var result1 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime).ToList()).ToListAsync();
        var result2 = await _tasksService.GetTasksForDays(dates.Select(DateOnly.FromDateTime).ToList()).ToListAsync();

        result1.Should().NotBeNull();
        result1.Should().HaveCount(dates.Count);
        result1.Should().Contain(x => x.TasksTimeline.Any(y => y.Task.Id == fixedTask.Id));

        result1.Should().BeEquivalentTo(result2, o => o.Excluding(x => x.Path.EndsWith("Task.CreatedTimestamp")));
    }


    #region Mock helpers

    private void SetupMocks(DateTime date, Guid userId)
    {
        _dynamicTasks =
        [
            new()
            {
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
                UserId = Guid.NewGuid(),
                Name = "TestDynamicTask3",
                Priority = 1,
                Description = "Test description",
                MinTimeToFinish = new TimeSpan(0, 30, 0),
                MaxTimeToFinish = new TimeSpan(1, 0, 0),
                OptimalTimeToFinish = new TimeSpan(0, 45, 0)
            }
        ];
        _dynamicTasksRepository.As<IUserScopedRepositoryBase<DynamicTask, Guid>>().SetupRepositoryMock(_dynamicTasks);

        _fixedTasks =
        [
            new()
            {
                UserId = userId,
                Name = "TestFixedTask1",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = date.AddHours(1),
                EndTimestamp = date.AddHours(1).AddMinutes(30)
            },

            new()
            {
                UserId = userId,
                Name = "TestFixedTask2",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = date.AddHours(2),
                EndTimestamp = date.AddHours(2).AddMinutes(30)
            },

            new()
            {
                UserId = Guid.NewGuid(),
                Name = "TestFixedTask3",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = date.AddHours(3),
                EndTimestamp = date.AddHours(3).AddMinutes(30)
            },

            new()
            {
                UserId = Guid.NewGuid(),
                Name = "TestFixedTask4",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = date.AddDays(-2).AddHours(3),
                EndTimestamp = date.AddHours(3).AddMinutes(30)
            }
        ];
        _fixedTasksRepository.As<IUserScopedRepositoryBase<FixedTask, Guid>>().SetupRepositoryMock(_fixedTasks);

        _scheduleSnapshots = new List<ScheduleSnapshot>();
        _scheduleSnapshotRepository.As<IUserScopedRepositoryBase<ScheduleSnapshot, Guid>>()
            .SetupRepositoryMock(_scheduleSnapshots);
        /*_scheduleSnapshotRepository.Setup(x => x.GetAll(It.IsAny<QueryPipelineStep<ScheduleSnapshot>[]>()))
            .Returns(_scheduleSnapshots.AsQueryable().BuildMock());
        _scheduleSnapshotRepository
            .Setup(x => x.GetAll(It.IsAny<bool>(), It.IsAny<QueryPipelineStep<ScheduleSnapshot>[]>()))
            .Returns(_scheduleSnapshots.AsQueryable().BuildMock());
        _scheduleSnapshotRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<ScheduleSnapshot>(), It.IsAny<CancellationToken>()))
            .Callback<ScheduleSnapshot, CancellationToken>((model, _) => { _scheduleSnapshots.Add(model); })
            .Returns<ScheduleSnapshot, CancellationToken>((model, _) => Task.FromResult(model));

        _scheduleSnapshotRepository.Setup(x => x.UpdateAndSaveAsync(It.IsAny<ScheduleSnapshot>(), It.IsAny<CancellationToken>()))
            .Callback<ScheduleSnapshot, CancellationToken>((model, _) =>
            {
                var existingObj =
                    _scheduleSnapshots.First(x => x.UserId == model.UserId && x.Date == model.Date);
                _scheduleSnapshots.Remove(existingObj);
                _scheduleSnapshots.Add(model);
            }).Returns<ScheduleSnapshot, CancellationToken>((model, _) => Task.FromResult(model));*/

        _scheduleEntities = [];

        _scheduleEntityRepository.As<IUserScopedRepositoryBase<ScheduleEntity, Guid>>().SetupRepositoryMock(_scheduleEntities);
    }

    #endregion
}