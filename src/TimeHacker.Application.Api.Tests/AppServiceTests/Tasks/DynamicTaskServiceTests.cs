namespace TimeHacker.Application.Api.Tests.AppServiceTests.Tasks;

public class DynamicTaskAppServiceTests
{
    #region Mocks

    private readonly Mock<IDynamicTaskRepository> _dynamicTasksRepository = new();

    #endregion

    #region Properties & constructor

    private List<DynamicTask> _dynamicTasks = null!;

    private readonly IDynamicTaskAppService _dynamicTaskAppService;
    private readonly Guid _userId = Guid.NewGuid();

    public DynamicTaskAppServiceTests()
    {
        SetupMocks(_userId);
        _dynamicTaskAppService = new DynamicTaskAppService(_dynamicTasksRepository.Object);
    }

    #endregion

    [Fact]
    [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
    public async Task AddAsync_ShouldAddEntry()
    {
        var newEntry = new DynamicTaskDto()
        {
            Name = "TestDynamicTask1000"
        };
        await _dynamicTaskAppService.AddAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _dynamicTasks.FirstOrDefault(x => x.Name == newEntry.Name);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should update entry")]
    public async Task UpdateAsync_ShouldUpdateEntry()
    {
        var newEntry = new DynamicTaskDto()
        {
            Id = _dynamicTasks.First(x => x.UserId == _userId).Id,
            Name = "TestDynamicTask1000"
        };
        await _dynamicTaskAppService.UpdateAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _dynamicTasks.FirstOrDefault(x => x.Id == newEntry.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAsync", "Should call update only once")]
    public async Task UpdateAsync_ShouldCallUpdateOnce()
    {
        var taskToUpdate = _dynamicTasks.First(x => x.UserId == _userId);
        var updateDto = new DynamicTaskDto
        {
            Id = taskToUpdate.Id,
            Name = "Updated Name",
            Priority = 2,
            MinTimeToFinish = new TimeSpan(0, 20, 0),
            MaxTimeToFinish = new TimeSpan(1, 30, 0),
            OptimalTimeToFinish = new TimeSpan(0, 50, 0)
        };

        await _dynamicTaskAppService.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        _dynamicTasksRepository.Verify(
            x => x.UpdateAndSaveAsync(It.IsAny<DynamicTask>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "UpdateAndSaveAsync should be called exactly once, but duplicate calls detected (bug at lines 29-30 in DynamicTaskService)");
    }

    [Fact]
    [Trait("UpdateAsync", "Should not double update entity")]
    public async Task UpdateAsync_ShouldNotDoubleUpdateEntity()
    {
        var taskToUpdate = _dynamicTasks.First(x => x.UserId == _userId);
        var originalName = taskToUpdate.Name;

        var updateDto = new DynamicTaskDto
        {
            Id = taskToUpdate.Id,
            Name = "Updated Name Once",
            Priority = 3
        };

        await _dynamicTaskAppService.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        var updatedTask = _dynamicTasks.First(x => x.Id == taskToUpdate.Id);
        updatedTask.Name.Should().Be("Updated Name Once");
        updatedTask.Priority.Should().Be(3);

        // Verify the task list doesn't have duplicates or corruption from double update
        _dynamicTasks.Count(x => x.Id == taskToUpdate.Id).Should().Be(1);
    }

    [Fact]
    [Trait("DeleteAndSaveAsync", "Should delete entry")]
    public async Task DeleteAsync_ShouldDeleteEntry()
    {
        var id = _dynamicTasks.First(x => x.UserId == _userId).Id;
        await _dynamicTaskAppService.DeleteAsync(id, TestContext.Current.CancellationToken);
        var result = _dynamicTasks.FirstOrDefault(x => x.Id == id);
        result.Should().BeNull();
    }

    [Fact]
    [Trait("GetAll", "Should return correct data")]
    public async Task GetAll_ShouldReturnCorrectData()
    {
        var result = await _dynamicTaskAppService.GetAll(TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        result.Count.Should().Be(_dynamicTasks.Count);
        result.Should().BeEquivalentTo(_dynamicTasks.Select(DynamicTaskDto.Create).ToList());
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return correct data")]
    public async Task GetByIdAsync_ShouldUpdateEntry()
    {
        var id = _dynamicTasks.First(x => x.UserId == _userId).Id;
        var result = await _dynamicTaskAppService.GetByIdAsync(id, TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    // Validation Tests
    [Fact]
    [Trait("AddAsync", "Should throw on null input")]
    public async Task AddAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _dynamicTaskAppService.AddAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("UpdateAsync", "Should throw on null input")]
    public async Task UpdateAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _dynamicTaskAppService.UpdateAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return null for non-existent ID")]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNonExistentId()
    {
        var result = await _dynamicTaskAppService.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);
        result.Should().BeNull();
    }

    #region Mock helpers

    private void SetupMocks(Guid userId)
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
    }

    #endregion
}
