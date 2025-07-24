using AwesomeAssertions;
using Moq;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.Services.ScheduleSnapshots;
using TimeHacker.Domain.Tests.Mocks.Extensions;

namespace TimeHacker.Domain.Tests.ServiceTests.ScheduleSnapshots
{
    public class ScheduledTaskServiceTests
    {
        #region Mocks

        private readonly Mock<IScheduledTaskRepository> _scheduledTaskRepository = new();

        #endregion

        #region Properties & constructor

        private List<ScheduledTask> _scheduledTasks;

        private readonly IScheduledTaskService _scheduledTaskService;
        private readonly Guid _userId = Guid.NewGuid();

        public ScheduledTaskServiceTests()
        {
            SetupMocks(_userId);
            _scheduledTaskService = new ScheduledTaskService(_scheduledTaskRepository.Object);
        }

        #endregion

        [Fact]
        [Trait("GetBy", "Should return correct data")]
        public async Task GetBy_ShouldReturnCorrectData()
        {
            var id = _scheduledTasks.First().Id;
            var result = await _scheduledTaskService.GetBy(id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
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

            _scheduledTaskRepository.As<IUserScopedRepositoryBase<ScheduledTask, Guid>>().SetupRepositoryMock(_scheduledTasks);
        }

        #endregion
    }
}
