namespace TimeHacker.Application.Api.Tests.AppServiceTests.ScheduleSnapshots;

public class ScheduledTaskAppServiceTests
{
    #region Mocks

    private readonly Mock<IScheduledTaskRepository> _scheduledTaskRepository = new();

    #endregion

    #region Properties & constructor

    private List<ScheduledTask> _scheduledTasks;

    private readonly IScheduledTaskAppService _scheduledTaskAppService;
    private readonly Guid _userId = Guid.NewGuid();

    public ScheduledTaskAppServiceTests()
    {
        SetupMocks(_userId);
        _scheduledTaskAppService = new ScheduledTaskAppService(_scheduledTaskRepository.Object);
    }

    #endregion

    [Fact]
    [Trait("GetBy", "Should return correct data")]
    public async Task GetBy_ShouldReturnCorrectData()
    {
        var id = _scheduledTasks.First().Id;
        var result = await _scheduledTaskAppService.GetBy(id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    [Trait("GetBy", "Should return null for non-existent ID")]
    public async Task GetBy_ShouldReturnNull_WhenNonExistentId()
    {
        var nonExistentId = Guid.NewGuid();

        var result = await _scheduledTaskAppService.GetBy(nonExistentId);

        result.Should().BeNull();
    }

    [Fact]
    [Trait("GetBy", "Should respect user scoping")]
    public async Task GetBy_ShouldRespectUserScoping()
    {
        var otherUserTask = _scheduledTasks.First(x => x.UserId != _userId);

        var result = await _scheduledTaskAppService.GetBy(otherUserTask.Id);

        result.Should().BeNull();
    }

    #region Mock helpers

    private void SetupMocks(Guid userId)
    {
        _scheduledTasks =
        [
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "TestFixedTask1",
                Date =  DateOnly.FromDateTime(DateTime.Now),
                Description = "Test description",
                ScheduleEntity = new ScheduleEntity()
            },

            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "TestFixedTask2",
                Date =  DateOnly.FromDateTime(DateTime.Now),
                Description = "Test description",
            },

            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "TestFixedTask3",
                Date =  DateOnly.FromDateTime(DateTime.Now),
                Description = "Test description",
                ScheduleEntity = new ScheduleEntity()
            },

            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "TestFixedTask4",
                Date =  DateOnly.FromDateTime(DateTime.Now),
                Description = "Test description",
            }
        ];

        _scheduledTaskRepository.As<IUserScopedRepositoryBase<ScheduledTask, Guid>>().SetupRepositoryMock(_scheduledTasks, userId);
    }

    #endregion
}
