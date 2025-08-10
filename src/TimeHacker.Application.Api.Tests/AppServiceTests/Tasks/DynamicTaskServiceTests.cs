using AwesomeAssertions;
using Moq;
using TimeHacker.Application.Api.AppServices.Tasks;
using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Helpers.Tests.Mocks.Extensions;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.Tasks
{
    public class DynamicTaskAppServiceTests
    {
        #region Mocks

        private readonly Mock<IDynamicTaskRepository> _dynamicTasksRepository = new();

        #endregion

        #region Properties & constructor

        private List<DynamicTask> _dynamicTasks;

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
            await _dynamicTaskAppService.AddAsync(newEntry);
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
            await _dynamicTaskAppService.UpdateAsync(newEntry);
            var result = _dynamicTasks.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldDeleteEntry()
        {
            var id = _dynamicTasks.First(x => x.UserId == _userId).Id;
            await _dynamicTaskAppService.DeleteAsync(id);
            var result = _dynamicTasks.FirstOrDefault(x => x.Id == id);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public async Task GetAll_ShouldReturnCorrectData()
        {
            var result = await _dynamicTaskAppService.GetAll().ToListAsync();

            result.Count.Should().Be(_dynamicTasks.Count);
            result.Should().BeEquivalentTo(_dynamicTasks.Select(DynamicTaskDto.Create).ToList());
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry()
        {
            var id = _dynamicTasks.First(x => x.UserId == _userId).Id;
            var result = await _dynamicTaskAppService.GetByIdAsync(id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
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
}
