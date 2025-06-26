using AwesomeAssertions;
using Moq;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.IServices.Tasks;
using TimeHacker.Domain.Services.Tasks;
using TimeHacker.Domain.Tests.Mocks;
using TimeHacker.Domain.Tests.Mocks.Extensions;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

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

        public FixedTaskServiceTests()
        {
            var userAccessor = new UserAccessorBaseMock("TestIdentifier", true);

            _fixedTaskService = new FixedTaskService(_fixedTasksRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var newEntry = new FixedTask()
            {
                Name = "TestFixedTask1000",
                UserId = "IncorrectUserId"
            };
            await _fixedTaskService.AddAsync(newEntry);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
            result!.UserId.Should().Be(userId);
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var newEntry = new FixedTask()
            {
                Id = _fixedTasks.First(x => x.UserId == userId).Id,
                Name = "TestFixedTask1000"
            };
            await _fixedTaskService.UpdateAsync(newEntry);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should throw exception on incorrect userId")]
        public async Task UpdateAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                var newEntry = new FixedTask()
                {
                    Id = _fixedTasks.First(x => x.UserId != userId).Id,
                    Name = "TestFixedTask1000"
                };
                await _fixedTaskService.UpdateAsync(newEntry);
                var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            });
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var idToDelete = _fixedTasks.First(x => x.UserId == userId).Id;
            await _fixedTaskService.DeleteAsync(idToDelete);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == idToDelete);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should throw exception on incorrect userId")]
        public async Task DeleteAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                await _fixedTaskService.DeleteAsync(_fixedTasks.First(x => x.UserId != userId).Id);
            });
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public void GetAll_ShouldReturnCorrectData()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var result = _fixedTaskService.GetAll().ToList();

            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(_fixedTasks.Where(x => x.UserId == userId).ToList());
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var id = _fixedTasks.First(x => x.UserId == userId).Id;
            var result = await _fixedTaskService.GetByIdAsync(id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return nothing on incorrect userId")]
        public async Task GetByIdAsync_ShouldThrow()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var result = await _fixedTaskService.GetByIdAsync(_fixedTasks.First(x => x.UserId != userId).Id);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("UpdateScheduleEntityAsync", "Should update schedule entry")]
        public async Task UpdateScheduleEntityAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var newEntry = new ScheduleEntity();

            var id = _fixedTasks.First(x => x.UserId == userId).Id;
            await _fixedTaskService.UpdateScheduleEntityAsync(newEntry, id);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == id);
            result.Should().NotBeNull();
            result!.ScheduleEntity.Should().NotBeNull();
            result!.ScheduleEntity!.Id.Should().Be(newEntry.Id);
        }

        [Fact]
        [Trait("UpdateScheduleEntityAsync", "Should throw exception on incorrect userId")]
        public async Task UpdateScheduleEntityAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                var newEntry = new ScheduleEntity();
                await _fixedTaskService.UpdateScheduleEntityAsync(newEntry, _fixedTasks.First(x => x.UserId != userId).Id);
            });
        }

        #region Mock helpers

        private void SetupMocks(string userId)
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
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask3",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddHours(3),
                    EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30),
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask4",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddDays(-2).AddHours(3),
                    EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30)
                }
            ];

            _fixedTasksRepository.As<IRepositoryBase<FixedTask, Guid>>().SetupRepositoryMock(_fixedTasks);
        }

        #endregion
    }
}
