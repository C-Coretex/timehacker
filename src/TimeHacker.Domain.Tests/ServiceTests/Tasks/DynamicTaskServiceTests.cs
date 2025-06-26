using AwesomeAssertions;
using Moq;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.IServices.Tasks;
using TimeHacker.Domain.Services.Tasks;
using TimeHacker.Domain.Tests.Mocks;
using TimeHacker.Domain.Tests.Mocks.Extensions;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Tests.ServiceTests.Tasks
{
    public class DynamicTaskServiceTests
    {
        #region Mocks

        private readonly Mock<IDynamicTaskRepository> _dynamicTasksRepository = new();

        #endregion

        #region Properties & constructor

        private List<DynamicTask> _dynamicTasks;

        private readonly IDynamicTaskService _dynamicTaskService;

        public DynamicTaskServiceTests()
        {
            var userAccessor = new UserAccessorBaseMock("TestIdentifier", true);

            _dynamicTaskService = new DynamicTaskService(_dynamicTasksRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var newEntry = new DynamicTask()
            {
                Name = "TestDynamicTask1000",
                UserId = "IncorrectUserId"
            };
            await _dynamicTaskService.AddAsync(newEntry);
            var result = _dynamicTasks.FirstOrDefault(x => x.Id == newEntry.Id);
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

            var newEntry = new DynamicTask()
            {
                Id = _dynamicTasks.First(x => x.UserId == userId).Id,
                Name = "TestDynamicTask1000"
            };
            await _dynamicTaskService.UpdateAsync(newEntry);
            var result = _dynamicTasks.FirstOrDefault(x => x.Id == newEntry.Id);
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

                var newEntry = new DynamicTask()
                {
                    Id = _dynamicTasks.First(x => x.UserId != userId).Id,
                    Name = "TestDynamicTask1000"
                };
                await _dynamicTaskService.UpdateAsync(newEntry);
            });
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldDeleteEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var id = _dynamicTasks.First(x => x.UserId == userId).Id;
            await _dynamicTaskService.DeleteAsync(id);
            var result = _dynamicTasks.FirstOrDefault(x => x.Id == id);
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

                var id = _dynamicTasks.First(x => x.UserId != userId).Id;
                await _dynamicTaskService.DeleteAsync(id);
            });
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public void GetAll_ShouldReturnCorrectData()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var result = _dynamicTaskService.GetAll().ToList();

            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(_dynamicTasks.Where(x => x.UserId == userId).ToList());
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var id = _dynamicTasks.First(x => x.UserId == userId).Id;
            var result = await _dynamicTaskService.GetByIdAsync(id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return nothing on incorrect userId")]
        public async Task GetByIdAsync_ShouldThrow()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var result = await _dynamicTaskService.GetByIdAsync(_dynamicTasks.First(x => x.UserId != userId).Id);
            result.Should().BeNull();
        }

        #region Mock helpers

        private void SetupMocks(string userId)
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
                    UserId = "IncorrectUserId",
                    Name = "TestDynamicTask3",
                    Priority = 1,
                    Description = "Test description",
                    MinTimeToFinish = new TimeSpan(0, 30, 0),
                    MaxTimeToFinish = new TimeSpan(1, 0, 0),
                    OptimalTimeToFinish = new TimeSpan(0, 45, 0)
                }
            ];

            _dynamicTasksRepository.As<IRepositoryBase<DynamicTask, Guid>>().SetupRepositoryMock(_dynamicTasks);
        }

        #endregion
    }
}
