using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Services.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Tests.Mocks;
using TimeHacker.Tests.Mocks.Extensions;

namespace TimeHacker.Tests.ServiceTests.Tasks
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
            var userAccessor = new UserAccessorMock("TestIdentifier", true);

            _fixedTaskService = new FixedTaskService(_fixedTasksRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var userId = "TestIdentifier";
            SetupFixedTaskMocks(userId);

            var newEntry = new FixedTask()
            {
                Id = 1000,
                Name = "TestFixedTask1000"
            };
            await _fixedTaskService.AddAsync(newEntry);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupFixedTaskMocks(userId);

            var newEntry = new FixedTask()
            {
                Id = 1,
                Name = "TestFixedTask1000"
            };
            await _fixedTaskService.UpdateAsync(newEntry);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
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
                SetupFixedTaskMocks(userId);

                var newEntry = new FixedTask()
                {
                    Id = 3,
                    Name = "TestFixedTask1000"
                };
                await _fixedTaskService.UpdateAsync(newEntry);
                var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            });
        }

        [Fact]
        [Trait("DeleteAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupFixedTaskMocks(userId);

            await _fixedTaskService.DeleteAsync(1);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == 1);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("DeleteAsync", "Should throw exception on incorrect userId")]
        public async Task DeleteAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupFixedTaskMocks(userId);

                await _fixedTaskService.DeleteAsync(3);
            });
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public void GetAll_ShouldReturnCorrectData()
        {
            var userId = "TestIdentifier";
            SetupFixedTaskMocks(userId);

            var result = _fixedTaskService.GetAll().ToList();

            result.Count.Should().Be(2);
            result.Select(x => x.Id).Should().BeEquivalentTo([1, 2]);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupFixedTaskMocks(userId);

            var result = await _fixedTaskService.GetByIdAsync(1);
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return nothing on incorrect userId")]
        public async Task GetByIdAsync_ShouldThrow()
        {
            var userId = "TestIdentifier";
            SetupFixedTaskMocks(userId);

            var result = await _fixedTaskService.GetByIdAsync(3);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("UpdateScheduleEntityAsync", "Should update schedule entry")]
        public async Task UpdateScheduleEntityAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupFixedTaskMocks(userId);

            var newEntry = new ScheduleEntity()
            {
                Id = 100
            };
            await _fixedTaskService.UpdateScheduleEntityAsync(newEntry, 1);
            var result = _fixedTasks.FirstOrDefault(x => x.Id == 1);
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
                SetupFixedTaskMocks(userId);

                var newEntry = new ScheduleEntity()
                {
                    Id = 1
                };
                await _fixedTaskService.UpdateScheduleEntityAsync(newEntry, 3);
            });
        }

        #region Mock helpers

        private void SetupFixedTaskMocks(string userId)
        {
            _fixedTasks =
            [
                new()
                {
                    Id = 1,
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
                    Id = 2,
                    UserId = userId,
                    Name = "TestFixedTask2",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddHours(2),
                    EndTimestamp = DateTime.Now.AddHours(2).AddMinutes(30)
                },

                new()
                {
                    Id = 3,
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
                    Id = 4,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask4",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddDays(-2).AddHours(3),
                    EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30)
                }
            ];

            _fixedTasksRepository.As<IRepositoryBase<FixedTask, uint>>().SetupRepositoryMock(_fixedTasks);
        }

        #endregion
    }
}
