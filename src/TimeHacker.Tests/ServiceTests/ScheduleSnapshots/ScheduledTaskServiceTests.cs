using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Tests.Mocks;

namespace TimeHacker.Tests.ServiceTests.ScheduleSnapshots
{
    public class ScheduledTaskServiceTests
    {
        #region Mocks

        private readonly Mock<IScheduledTaskRepository> _scheduledTaskRepository = new();

        private readonly IUserAccessor userAccessor;

        #endregion

        #region Properties & constructor

        private List<ScheduledTask> _scheduledCategories;

        private readonly IScheduledTaskService _scheduledTaskService;

        public ScheduledTaskServiceTests()
        {
            var userAccessor = new UserAccessorMock("TestIdentifier", true);

            _scheduledTaskService = new ScheduledTaskService(_scheduledTaskRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("GetBy", "Should return correct data")]
        public async Task DeleteAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupFixedTaskMocks(userId);

            var result = await _scheduledTaskService.GetBy(1);
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        [Trait("GetBy", "Should throw exception on incorrect userId")]
        public async Task DeleteAsync_ShouldThrow()
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
            _scheduledCategories =
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

            _scheduledTaskRepository.Setup(x => x.AddAsync(It.IsAny<ScheduledTask>(), It.IsAny<bool>()))
                .Callback<ScheduledTask, bool>((entry, _) => _scheduledCategories.Add(entry));

            _scheduledTaskRepository.Setup(x => x.UpdateAsync(It.IsAny<ScheduledTask>(), It.IsAny<bool>()))
                .Callback<ScheduledTask, bool>((entry, _) =>
                {
                    _scheduledCategories.RemoveAll(x => x.Id == entry.Id);
                    _scheduledCategories.Add(entry);
                })
                .Returns<ScheduledTask, bool>((entry, _) => Task.FromResult(entry));

            _scheduledTaskRepository.Setup(x => x.GetByIdAsync(It.IsAny<uint>(), It.IsAny<bool>(), It.IsAny<IncludeExpansionDelegate<ScheduledTask>[]>()))
                .Returns<uint, bool, IncludeExpansionDelegate<ScheduledTask>[]>((id, _, _) => Task.FromResult(_scheduledCategories.FirstOrDefault(x => x.Id == id)));

            _scheduledTaskRepository.Setup(x => x.DeleteAsync(It.IsAny<ScheduledTask>(), It.IsAny<bool>()))
                .Callback<ScheduledTask, bool>((entry, _) => _scheduledCategories.RemoveAll(x => x.Id == entry.Id));

            _scheduledTaskRepository.Setup(x => x.GetAll(It.IsAny<bool>()))
                .Returns(_scheduledCategories.AsQueryable().BuildMock());
        }

        #endregion
    }
}
