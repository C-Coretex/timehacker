using AwesomeAssertions;
using Moq;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.IServices.Tasks;
using TimeHacker.Domain.Services.Services.Tasks;
using TimeHacker.Domain.Tests.Mocks.Extensions;

namespace TimeHacker.Domain.Tests.ServiceTests.Tasks
{
    public class FixedTaskServiceTests
    {
        #region Mocks

        private readonly Mock<IFixedTaskRepository> _fixedTasksRepository = new();

        #endregion

        #region Properties & constructor

        private List<FixedTask> _fixedTasks;

        private readonly IFixedTaskService _fixedTaskService;
        private readonly Guid _userId = Guid.NewGuid();

        public FixedTaskServiceTests()
        {
            SetupMocks(_userId);
            _fixedTaskService = new FixedTaskService(_fixedTasksRepository.Object);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var newEntry = new FixedTask()
            {
                Name = "TestFixedTask1000",
                UserId = Guid.NewGuid()
            };
            await _fixedTaskService.AddAsync(newEntry);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var newEntry = new FixedTask()
            {
                Id = _fixedTasks.First(x => x.UserId == _userId).Id,
                Name = "TestFixedTask1000"
            };
            await _fixedTaskService.UpdateAsync(newEntry);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldUpdateEntry()
        {
            var idToDelete = _fixedTasks.First(x => x.UserId == _userId).Id;
            await _fixedTaskService.DeleteAsync(idToDelete);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == idToDelete);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public async Task GetAll_ShouldReturnCorrectData()
        {
            var result = await _fixedTaskService.GetAll().ToListAsync();

            result.Count.Should().Be(_fixedTasks.Count);
            result.Should().BeEquivalentTo(_fixedTasks.ToList());
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry()
        {
            var id = _fixedTasks.First(x => x.UserId == _userId).Id;
            var result = await _fixedTaskService.GetByIdAsync(id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        [Trait("UpdateScheduleEntityAsync", "Should update schedule entry")]
        public async Task UpdateScheduleEntityAsync_ShouldUpdateEntry()
        {
            var newEntry = new ScheduleEntity();

            var id = _fixedTasks.First(x => x.UserId == _userId).Id;
            await _fixedTaskService.UpdateScheduleEntityAsync(newEntry, id);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == id);
            result.Should().NotBeNull();
            result!.ScheduleEntity.Should().NotBeNull();
            result!.ScheduleEntity!.Id.Should().Be(newEntry.Id);
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
}
