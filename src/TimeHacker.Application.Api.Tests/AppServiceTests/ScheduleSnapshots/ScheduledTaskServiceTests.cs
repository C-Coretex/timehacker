namespace TimeHacker.Application.Api.Tests.AppServiceTests.ScheduleSnapshots
{
    public class ScheduledTaskAppServiceTests
    {
        #region Mocks

        private readonly Mock<IScheduledTaskRepository> _scheduledTaskRepository = new();

        #endregion

        #region Properties & constructor

        private List<ScheduledTask> _scheduledTasks;

        private readonly IScheduledTaskAppService _scheduledTaskAppService;
        private readonly Guid _userId = Guid.NewGuid();

        public ScheduledTaskAppServiceTests()
        {
            SetupMocks(_userId);
            _scheduledTaskAppService = new ScheduledTaskAppService(_scheduledTaskRepository.Object);
        }

        #endregion

        [Fact]
        [Trait("GetBy", "Should return correct data")]
        public async Task GetBy_ShouldReturnCorrectData()
        {
            var id = _scheduledTasks.First().Id;
            var result = await _scheduledTaskAppService.GetBy(id);
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
