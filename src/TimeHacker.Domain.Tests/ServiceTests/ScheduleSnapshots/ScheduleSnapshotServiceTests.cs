using AwesomeAssertions;
using Moq;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Domain.Tests.Mocks;
using TimeHacker.Domain.Tests.Mocks.Extensions;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Tests.ServiceTests.ScheduleSnapshots
{
    public class ScheduleSnapshotServiceTests
    {
        #region Mocks

        private readonly Mock<IScheduleSnapshotRepository> _scheduleSnapshotRepository = new();

        #endregion


        #region Properties & constructor

        private List<ScheduleSnapshot> _scheduleSnapshots;

        private readonly IScheduleSnapshotService _scheduleSnapshotService;

        public ScheduleSnapshotServiceTests()
        {
            var userAccessor = new UserAccessorBaseMock("TestIdentifier", true);

            _scheduleSnapshotService = new ScheduleSnapshotService(_scheduleSnapshotRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct data")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var date = DateOnly.FromDateTime(DateTime.Now);
            var lastUpdateTimestamp = DateTime.Now;
            var newEntry = new ScheduleSnapshot()
            {
                UserId = userId,
                Date = date,
                LastUpdateTimestamp = lastUpdateTimestamp,
                ScheduledCategories = [new(), new(), new()],
                ScheduledTasks = [new(), new(), new()]
            };

            await Task.Delay(100);
            var actual = await _scheduleSnapshotService.AddAsync(newEntry);
            var actual2 = _scheduleSnapshots.FirstOrDefault(x => x.UserId == userId && x.Date == date);

            actual2.Should().NotBeNull();
            actual.Should().Be(actual2);

            actual.LastUpdateTimestamp.Should().NotBe(lastUpdateTimestamp);
            actual.ScheduledCategories.Should().AllSatisfy(x =>
            {
                x.Date.Should().Be(newEntry.Date);
                x.UserId.Should().Be(newEntry.UserId);
                x.UpdatedTimestamp.Should().Be(actual.LastUpdateTimestamp);
            });
            actual.ScheduledCategories.Count.Should().Be(3);
            actual.ScheduledTasks.Should().AllSatisfy(x =>
            {
                x.Date.Should().Be(newEntry.Date);
                x.UserId.Should().Be(newEntry.UserId);
                x.UpdatedTimestamp.Should().Be(actual.LastUpdateTimestamp);
            });
            actual.ScheduledTasks.Count.Should().Be(3);
        }

        [Theory, CombinatorialData]
        [Trait("GetByAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry(bool correctUser)
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(correctUser ? -1 : 0));
            var actual = await _scheduleSnapshotService.GetByAsync(date);
            if (correctUser)
            {
                actual.Should().NotBeNull();

                var expected = _scheduleSnapshots.First(x => x.UserId == userId && x.Date == date);
                actual.Should().Be(expected);
            }
            else
                actual.Should().BeNull();
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldAddEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var date = DateOnly.FromDateTime(DateTime.Now);
            var lastUpdateTimestamp = DateTime.Now;
            var newEntry = new ScheduleSnapshot()
            {
                UserId = userId,
                Date = date,
                LastUpdateTimestamp = lastUpdateTimestamp,
                ScheduledCategories = [new(), new(), new()],
                ScheduledTasks = [new(), new(), new()]
            };

            await Task.Delay(100);
            var actual = await _scheduleSnapshotService.UpdateAsync(newEntry);
            var actual2 = _scheduleSnapshots.FirstOrDefault(x => x.UserId == userId && x.Date == date);

            actual2.Should().NotBeNull();
            actual.Should().Be(actual2);

            actual.LastUpdateTimestamp.Should().NotBe(lastUpdateTimestamp);
            actual.ScheduledCategories.Should().AllSatisfy(x =>
            {
                x.Date.Should().Be(newEntry.Date);
                x.UserId.Should().Be(newEntry.UserId);
                x.UpdatedTimestamp.Should().Be(actual.LastUpdateTimestamp);
            });
            actual.ScheduledCategories.Count.Should().Be(3);
            actual.ScheduledTasks.Should().AllSatisfy(x =>
            {
                x.Date.Should().Be(newEntry.Date);
                x.UserId.Should().Be(newEntry.UserId);
                x.UpdatedTimestamp.Should().Be(actual.LastUpdateTimestamp);
            });
            actual.ScheduledTasks.Count.Should().Be(3);
        }

        #region Mock helpers

        private void SetupMocks(string userId)
        {
            _scheduleSnapshots =
            [
                new()
                {
                    UserId = userId,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                    LastUpdateTimestamp = DateTime.Now.AddHours(-4),
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()]
                },

                new()
                {
                    UserId = userId,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                    LastUpdateTimestamp = DateTime.Now.AddHours(-4),
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()]
                },

                new()
                {
                    UserId = "IncorrectUserId",
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    LastUpdateTimestamp = DateTime.Now.AddHours(-4),
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()]
                },

                new()
                {
                    UserId = "IncorrectUserId",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                    LastUpdateTimestamp = DateTime.Now.AddHours(-4),
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()]
                },
            ];

            _scheduleSnapshotRepository.As<IRepositoryBase<ScheduleSnapshot>>().SetupRepositoryMock(_scheduleSnapshots);

            _scheduleSnapshotRepository.Setup(x => x.UpdateAndSaveAsync(It.IsAny<ScheduleSnapshot>(), It.IsAny<CancellationToken>()))
                .Callback<ScheduleSnapshot, CancellationToken>((entry, _) =>
                {
                    _scheduleSnapshots.RemoveAll(x => x.UserId!.Equals(entry.UserId) && x.Date!.Equals(entry.Date));
                    _scheduleSnapshots.Add(entry);
                })
                .Returns<ScheduleSnapshot, CancellationToken>((entry, _) => Task.FromResult(entry));
        }

        #endregion
    }
}
