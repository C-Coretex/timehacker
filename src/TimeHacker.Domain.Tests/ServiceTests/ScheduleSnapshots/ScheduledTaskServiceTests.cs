using AwesomeAssertions;
using Moq;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.Services.ScheduleSnapshots;
using TimeHacker.Domain.Tests.Mocks;
using TimeHacker.Domain.Tests.Mocks.Extensions;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

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
            var userAccessor = new UserAccessorBaseMock(_userId, true);
            SetupMocks(_userId);
            _scheduledTaskService = new ScheduledTaskService(_scheduledTaskRepository.Object, userAccessor);
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

        [Fact]
        [Trait("GetBy", "Should throw exception on incorrect userId")]
        public async Task GetBy_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var id = _scheduledTasks.First(x => x.UserId != _userId).Id;
                var result = await _scheduledTaskService.GetBy(id);
            });
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
