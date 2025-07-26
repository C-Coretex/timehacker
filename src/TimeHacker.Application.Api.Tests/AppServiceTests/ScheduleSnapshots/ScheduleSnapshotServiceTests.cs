using AwesomeAssertions;
using Moq;
using TimeHacker.Application.Api.AppServices.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.Tests.Mocks;
using TimeHacker.Helpers.Tests.Mocks.Extensions;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.ScheduleSnapshots
{
    public class ScheduleSnapshotServiceTests
    {
        #region Mocks

        private readonly Mock<IScheduleSnapshotRepository> _scheduleSnapshotRepository = new();

        #endregion


        #region Properties & constructor

        private List<ScheduleSnapshot> _scheduleSnapshots;

        private readonly IScheduleSnapshotAppService _scheduleSnapshotService;
        private readonly Guid _userId = Guid.NewGuid();
        public ScheduleSnapshotServiceTests()
        {
            var userAccessor = new UserAccessorBaseMock(_userId, true);
            SetupMocks(_userId);
            _scheduleSnapshotService = new ScheduleSnapshotService(_scheduleSnapshotRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct data")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var date = DateOnly.FromDateTime(DateTime.Now);
            var newEntry = new ScheduleSnapshot()
            {
                UserId = _userId,
                Date = date,
                ScheduledCategories = [new() { Name = "" }, new() { Name = "" }, new() { Name = "" }],
                ScheduledTasks = [new() { Name = "" }, new() { Name = "" }, new() { Name = "" }]
            };

            await Task.Delay(100);
            var actual = await _scheduleSnapshotService.AddAsync(newEntry);
            var actual2 = _scheduleSnapshots.FirstOrDefault(x => x.UserId == _userId && x.Date == date);

            actual2.Should().NotBeNull();
            actual.Should().Be(actual2);

            actual.ScheduledCategories.Should().AllSatisfy(x =>
            {
                x.Date.Should().Be(newEntry.Date);
                x.UserId.Should().Be(newEntry.UserId);
            });
            actual.ScheduledCategories.Count.Should().Be(3);
            actual.ScheduledTasks.Should().AllSatisfy(x =>
            {
                x.Date.Should().Be(newEntry.Date);
                x.UserId.Should().Be(newEntry.UserId);
            });
            actual.ScheduledTasks.Count.Should().Be(3);
        }

        [Theory, CombinatorialData]
        [Trait("GetByAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry(bool correctUser)
        {
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(correctUser ? -1 : 1));
            var actual = await _scheduleSnapshotService.GetByAsync(date);
            if (correctUser)
            {
                actual.Should().NotBeNull();

                var expected = _scheduleSnapshots.First(x => x.UserId == _userId && x.Date == date);
                actual.Should().Be(expected);
            }
            else
                actual.Should().BeNull();
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldAddEntry()
        {
            var date = DateOnly.FromDateTime(DateTime.Now);
            var newEntry = new ScheduleSnapshot()
            {
                UserId = _userId,
                Date = date,
                ScheduledCategories = [new() { Name = "" }, new() { Name = "" }, new() { Name = "" }],
                ScheduledTasks = [new() { Name = "" }, new() { Name = "" }, new() { Name = "" }]
            };

            await Task.Delay(100);
            var actual = await _scheduleSnapshotService.UpdateAsync(newEntry);
            var actual2 = _scheduleSnapshots.FirstOrDefault(x => x.UserId == _userId && x.Date == date);

            actual2.Should().NotBeNull();
            actual.Should().Be(actual2);

            actual.ScheduledCategories.Should().AllSatisfy(x =>
            {
                x.Date.Should().Be(newEntry.Date);
                x.UserId.Should().Be(newEntry.UserId);
            });
            actual.ScheduledCategories.Count.Should().Be(3);
            actual.ScheduledTasks.Should().AllSatisfy(x =>
            {
                x.Date.Should().Be(newEntry.Date);
                x.UserId.Should().Be(newEntry.UserId);
            });
            actual.ScheduledTasks.Count.Should().Be(3);
        }

        #region Mock helpers

        private void SetupMocks(Guid userId)
        {
            _scheduleSnapshots =
            [
                new()
                {
                    UserId = userId,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                    ScheduledTasks = [new ScheduledTask() { Name = "" }],
                    ScheduledCategories = [new ScheduledCategory() { Name = "" }]
                },

                new()
                {
                    UserId = userId,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                    ScheduledTasks = [new ScheduledTask() { Name = "" }],
                    ScheduledCategories = [new ScheduledCategory() { Name = "" }]
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    ScheduledTasks = [new ScheduledTask() { Name = "" }],
                    ScheduledCategories = [new ScheduledCategory() { Name = "" }]
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                    ScheduledTasks = [new ScheduledTask() { Name = "" }],
                    ScheduledCategories = [new ScheduledCategory() { Name = "" }]
                },
            ];

            _scheduleSnapshotRepository.As<IUserScopedRepositoryBase<ScheduleSnapshot, Guid>>().SetupRepositoryMock(_scheduleSnapshots);

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
