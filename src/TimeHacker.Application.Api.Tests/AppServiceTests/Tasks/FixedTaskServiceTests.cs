namespace TimeHacker.Application.Api.Tests.AppServiceTests.Tasks;

public class FixedTaskAppServiceTests
{
    #region Mocks

    private readonly Mock<IFixedTaskRepository> _fixedTasksRepository = new();

    #endregion

    #region Properties & constructor

    private List<FixedTask> _fixedTasks;

    private readonly IFixedTaskAppService _fixedTaskAppService;
    private readonly Guid _userId = Guid.NewGuid();

    public FixedTaskAppServiceTests()
    {
        SetupMocks(_userId);
        _fixedTaskAppService = new FixedTaskAppService(_fixedTasksRepository.Object);
    }

    #endregion

    [Fact]
    [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
    public async Task AddAsync_ShouldAddEntry()
    {
        var newEntry = new FixedTaskDto()
        {
            Name = "TestFixedTask1000"
        };
        await _fixedTaskAppService.AddAsync(newEntry);
        var result = _fixedTasks.FirstOrDefault(x => x.Name == newEntry.Name);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should update entry")]
    public async Task UpdateAsync_ShouldUpdateEntry()
    {
        var newEntry = new FixedTaskDto()
        {
            Id = _fixedTasks.First(x => x.UserId == _userId).Id,
            Name = "TestFixedTask1000"
        };
        await _fixedTaskAppService.UpdateAsync(newEntry);
        var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("DeleteAndSaveAsync", "Should delete entry")]
    public async Task DeleteAsync_ShouldUpdateEntry()
    {
        var idToDelete = _fixedTasks.First(x => x.UserId == _userId).Id;
        await _fixedTaskAppService.DeleteAsync(idToDelete);
        var result = _fixedTasks.FirstOrDefault(x => x.Id == idToDelete);
        result.Should().BeNull();
    }

    [Fact]
    [Trait("GetAll", "Should return correct data")]
    public async Task GetAll_ShouldReturnCorrectData()
    {
        var result = await _fixedTaskAppService.GetAll().ToListAsync();

        result.Count.Should().Be(_fixedTasks.Count);
        result.Should().BeEquivalentTo(_fixedTasks.Select(FixedTaskDto.Create).ToList());
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return correct data")]
    public async Task GetByIdAsync_ShouldUpdateEntry()
    {
        var id = _fixedTasks.First(x => x.UserId == _userId).Id;
        var result = await _fixedTaskAppService.GetByIdAsync(id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    #region Mock helpers

    private void SetupMocks(Guid userId)
    {
        _fixedTasks =
        [
            new()
            {
                UserId = userId,
                Name = "TestFixedTask1",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = DateTime.Now.AddHours(1),
                EndTimestamp = DateTime.Now.AddHours(1).AddMinutes(30),
                ScheduleEntity = new ScheduleEntity()
            },

            new()
            {
                UserId = userId,
                Name = "TestFixedTask2",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = DateTime.Now.AddHours(2),
                EndTimestamp = DateTime.Now.AddHours(2).AddMinutes(30)
            },

            new()
            {
                UserId = Guid.NewGuid(),
                Name = "TestFixedTask3",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = DateTime.Now.AddHours(3),
                EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30),
                ScheduleEntity = new ScheduleEntity()
            },

            new()
            {
                UserId = Guid.NewGuid(),
                Name = "TestFixedTask4",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = DateTime.Now.AddDays(-2).AddHours(3),
                EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30)
            }
        ];

        _fixedTasksRepository.As<IUserScopedRepositoryBase<FixedTask, Guid>>().SetupRepositoryMock(_fixedTasks);
    }

    #endregion
}
