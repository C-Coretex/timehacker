using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Services.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Tests.Mocks;
using TimeHacker.Tests.Mocks.Extensions;

namespace TimeHacker.Tests.ServiceTests.Tasks
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
            var userAccessor = new UserAccessorMock("TestIdentifier", true);

            _dynamicTaskService = new DynamicTaskService(_dynamicTasksRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var userId = "TestIdentifier";
            SetupDynamicTaskMocks(userId);

            var newEntry = new DynamicTask()
            {
                Id = 1000,
                Name = "TestDynamicTask1000"
            };
            await _dynamicTaskService.AddAsync(newEntry);
            var result = _dynamicTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupDynamicTaskMocks(userId);

            var newEntry = new DynamicTask()
            {
                Id = 1,
                Name = "TestDynamicTask1000"
            };
            await _dynamicTaskService.UpdateAsync(newEntry);
            var result = _dynamicTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAsync", "Should throw exception on incorrect userId")]
        public async Task UpdateAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupDynamicTaskMocks(userId);

                var newEntry = new DynamicTask()
                {
                    Id = 3,
                    Name = "TestDynamicTask1000"
                };
                await _dynamicTaskService.UpdateAsync(newEntry);
                var result = _dynamicTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            });
        }

        [Fact]
        [Trait("DeleteAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldDeleteEntry()
        {
            var userId = "TestIdentifier";
            SetupDynamicTaskMocks(userId);

            await _dynamicTaskService.DeleteAsync(1);
            var result = _dynamicTasks.FirstOrDefault(x => x.Id == 1);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("DeleteAsync", "Should throw exception on incorrect userId")]
        public async Task DeleteAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupDynamicTaskMocks(userId);

                await _dynamicTaskService.DeleteAsync(3);
            });
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public void GetAll_ShouldReturnCorrectData()
        {
            var userId = "TestIdentifier";
            SetupDynamicTaskMocks(userId);

            var result = _dynamicTaskService.GetAll().ToList();

            result.Count.Should().Be(2);
            result.Select(x => x.Id).Should().BeEquivalentTo([1, 2]);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupDynamicTaskMocks(userId);

            var result = await _dynamicTaskService.GetByIdAsync(1);
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return nothing on incorrect userId")]
        public async Task GetByIdAsync_ShouldThrow()
        {
            var userId = "TestIdentifier";
            SetupDynamicTaskMocks(userId);

            var result = await _dynamicTaskService.GetByIdAsync(3);
            result.Should().BeNull();
        }

        #region Mock helpers

        private void SetupDynamicTaskMocks(string userId)
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

            _dynamicTasksRepository.As<IRepositoryBase<DynamicTask, uint>>().SetupRepositoryMock(_dynamicTasks);
        }

        #endregion
    }
}
