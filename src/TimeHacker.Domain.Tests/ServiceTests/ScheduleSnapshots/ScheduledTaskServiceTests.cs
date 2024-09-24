using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Tests.Mocks;
using TimeHacker.Tests.Mocks.Extensions;

namespace TimeHacker.Tests.ServiceTests.ScheduleSnapshots
{
    public class ScheduledTaskServiceTests
    {
        #region Mocks

        private readonly Mock<IScheduledTaskRepository> _scheduledTaskRepository = new();

        #endregion

        #region Properties & constructor

        private List<ScheduledTask> _scheduledTasks;

        private readonly IScheduledTaskService _scheduledTaskService;

        public ScheduledTaskServiceTests()
        {
            var userAccessor = new UserAccessorMock("TestIdentifier", true);

            _scheduledTaskService = new ScheduledTaskService(_scheduledTaskRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("GetBy", "Should return correct data")]
        public async Task GetBy_ShouldReturnCorrectData()
        {
            var userId = "TestIdentifier";
            SetupFixedTaskMocks(userId);

            var result = await _scheduledTaskService.GetBy(1);
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        [Trait("GetBy", "Should throw exception on incorrect userId")]
        public async Task GetBy_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupFixedTaskMocks(userId);

                var result = await _scheduledTaskService.GetBy(3);
            });
        }

        #region Mock helpers

        private void SetupFixedTaskMocks(string userId)
        {
            _scheduledTasks =
            [
                new()
                {
                    Id = 1,
                    UserId = userId,
                    Name = "TestFixedTask1",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    Id = 2,
                    UserId = userId,
                    Name = "TestFixedTask2",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                },

                new()
                {
                    Id = 3,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask3",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    Id = 4,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask4",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                }
            ];

            _scheduledTaskRepository.As<IRepositoryBase<ScheduledTask, ulong>>().SetupRepositoryMock(_scheduledTasks);
        }

        #endregion
    }
}
