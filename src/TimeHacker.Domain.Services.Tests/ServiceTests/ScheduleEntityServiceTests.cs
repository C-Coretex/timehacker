namespace TimeHacker.Domain.Services.Tests.ServiceTests;

public class ScheduleEntityServiceTests
{
    #region Mocks

    private readonly Mock<IScheduleEntityRepository> _scheduleEntityRepository = new();

    #endregion

    #region Properties & constructor

    private List<ScheduleEntity> _scheduledEntities = null!;

    private readonly Guid _userId = Guid.NewGuid();

    private readonly IScheduleEntityService _scheduleEntityService;

    public ScheduleEntityServiceTests()
    {
        SetupMocks(_userId);
        _scheduleEntityService = new ScheduleEntityService(_scheduleEntityRepository.Object);
    }

    #endregion
   
    [Fact]
    [Trait("GetAllFrom", "Should return correct data")]
    public void GetAllFrom_ShouldReturnCorrectData()
    {
        var date = DateTime.Now;

        _scheduledEntities.Clear();
        _scheduledEntities.AddRange(
        [
            new()
            {
                UserId = _userId,
                CreatedTimestamp = date,
                ScheduledTasks = [new ScheduledTask() { Name = "" }],
                ScheduledCategories = [new ScheduledCategory() { Name = "" }],
                EndsOn = null,
                FixedTask = new FixedTask() { UserId = _userId }
            },

            new()
            {
                UserId = _userId,
                CreatedTimestamp = date.AddDays(-1),
                EndsOn = DateOnly.FromDateTime(date),
                FixedTask = new FixedTask() { UserId = _userId }
            },

            new()
            {
                UserId = _userId,
                CreatedTimestamp = date.AddDays(-1).AddMinutes(10),
                EndsOn = DateOnly.FromDateTime(date.AddDays(-1).AddMinutes(10)),
                FixedTask = new FixedTask() { UserId = _userId }
            },

            new()
            {
                UserId = _userId,
                CreatedTimestamp = date.AddDays(-2),
                EndsOn = DateOnly.FromDateTime(date.AddDays(-2)),
                FixedTask = new FixedTask() { UserId = _userId }
            },

            new()
            {
                UserId = Guid.NewGuid(),
                CreatedTimestamp = date,
                ScheduledTasks = [new ScheduledTask() { Name = "" }],
                ScheduledCategories = [new ScheduledCategory() { Name = "" }],
                EndsOn = null,
                FixedTask = new FixedTask() { UserId = Guid.NewGuid() }
            },

            new()
            {
                UserId = Guid.NewGuid(),
                CreatedTimestamp = date.AddDays(-1),
                EndsOn = DateOnly.FromDateTime(date),
                FixedTask = new FixedTask() { UserId = Guid.NewGuid() }
            },

            new()
            {
                UserId = Guid.NewGuid(),
                CreatedTimestamp = date.AddDays(-2),
                EndsOn = DateOnly.FromDateTime(date.AddDays(-2)),
                FixedTask = new FixedTask() { UserId = Guid.NewGuid() }
            }
        ]);

        var from = DateOnly.FromDateTime(date.AddDays(-1).AddMinutes(-1));
        var actual = _scheduleEntityService.GetAllFrom(from).ToList();
        actual.Should().NotBeNull();

        var expected = _scheduledEntities.Where(x => x.FixedTask != null && (x.EndsOn == null || x.EndsOn >= from))
            .ToList();
        actual.Count.Should().Be(expected.Count);
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    [Trait("UpdateLastEntityCreated", "Should update data")]
    public async Task UpdateLastEntityCreated_ShouldUpdateData()
    {
        var lastEntryCreated = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
        var scheduleEntityId = _scheduledEntities.First(x => x.UserId == _userId).Id;
        await _scheduleEntityService.UpdateLastEntityCreated(scheduleEntityId, lastEntryCreated, TestContext.Current.CancellationToken);
        var actual = _scheduledEntities.First(x => x.Id == scheduleEntityId);
        actual.LastEntityCreated.Should().Be(lastEntryCreated);
    }

    [Theory, CombinatorialData]
    [Trait("UpdateLastEntityCreated", "Should throw exception on incorrect data")]
    public async Task UpdateLastEntityCreated_ShouldThrow(bool existingEntry)
    {
        if (!existingEntry)
        {
            await _scheduleEntityService.UpdateLastEntityCreated(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), TestContext.Current.CancellationToken);
            return;
        }

        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _scheduleEntityService.UpdateLastEntityCreated(_scheduledEntities.First(x => x.UserId != _userId).Id, DateOnly.FromDateTime(DateTime.Now), TestContext.Current.CancellationToken);
        });
    }


    [Fact]
    [Trait("UpdateLastEntityCreated", "Should throw exception on incorrect entity created data")]
    public async Task UpdateLastEntityCreated_ShouldThrowOnIncorrectEntityCreated()
    {
        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _scheduleEntityService.UpdateLastEntityCreated(
                _scheduledEntities.First(x => x.UserId == _userId).Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                TestContext.Current.CancellationToken);
        });
    }

    [Fact]
    [Trait("UpdateLastEntityCreated", "Should throw on invalid future date")]
    public async Task UpdateLastEntityCreated_ShouldThrowOnFutureDate()
    {
        var entity = _scheduledEntities.First(x => x.UserId == _userId && x.RepeatingEntity != null);

        // Try to set a date that doesn't match the repeating pattern (odd number of days, pattern requires even)
        var invalidDate = DateOnly.FromDateTime(DateTime.Now.AddDays(101));

        await Assert.ThrowsAsync<DataIsNotCorrectException>(async () =>
        {
            await _scheduleEntityService.UpdateLastEntityCreated(entity.Id, invalidDate, TestContext.Current.CancellationToken);
        });
    }

    [Fact]
    [Trait("UpdateLastEntityCreated", "Should set FirstEntityCreated when null")]
    public async Task UpdateLastEntityCreated_ShouldSetFirstEntityCreatedWhenNull()
    {
        var entity = _scheduledEntities.First(x => x.UserId == _userId && x.RepeatingEntity != null);
        entity.FirstEntityCreated = null;
        entity.LastEntityCreated = null;

        // Valid date according to the repeating pattern (2 days after CreatedTimestamp)
        var validDate = DateOnly.FromDateTime(entity.CreatedTimestamp.AddDays(2));

        await _scheduleEntityService.UpdateLastEntityCreated(entity.Id, validDate, TestContext.Current.CancellationToken);

        var updated = _scheduledEntities.First(x => x.Id == entity.Id);
        updated.FirstEntityCreated.Should().Be(validDate);
        updated.LastEntityCreated.Should().Be(validDate);
    }

    [Fact]
    [Trait("UpdateLastEntityCreated", "Should not update when date is earlier")]
    public async Task UpdateLastEntityCreated_ShouldNotUpdateWhenLastEntityCreatedIsLater()
    {
        var entity = _scheduledEntities.First(x => x.UserId == _userId && x.RepeatingEntity != null);
        var futureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
        entity.LastEntityCreated = futureDate;

        // Try to update with an earlier date
        var earlierDate = DateOnly.FromDateTime(DateTime.Now.AddDays(4));

        await _scheduleEntityService.UpdateLastEntityCreated(entity.Id, earlierDate, TestContext.Current.CancellationToken);

        var updated = _scheduledEntities.First(x => x.Id == entity.Id);
        updated.LastEntityCreated.Should().Be(futureDate);
    }


    #region Mock helpers

    private void SetupMocks(Guid userId)
    {
        _scheduledEntities =
        [
            new()
            {
                UserId = userId,
                CreatedTimestamp = DateTime.Now,
                RepeatingEntity = new RepeatingEntityDto(RepeatingEntityTypeEnum.DayRepeatingEntity, new DayRepeatingEntity(2)), 
                ScheduledTasks = [new ScheduledTask() { Name = "" }],
                ScheduledCategories = [new ScheduledCategory() { Name = "" }]
            },

            new()
            {
                UserId = userId,
                CreatedTimestamp = DateTime.Now,
            },

            new()
            {
                UserId = Guid.NewGuid(),
                CreatedTimestamp = DateTime.Now,
                ScheduledTasks = [new ScheduledTask() { Name = "" }],
                ScheduledCategories = [new ScheduledCategory() { Name = "" }]
            },

            new()
            {
                UserId = Guid.NewGuid(),
                CreatedTimestamp = DateTime.Now,
            }
        ];

        _scheduleEntityRepository.As<IUserScopedRepositoryBase<ScheduleEntity, Guid>>().SetupRepositoryMock(_scheduledEntities);
    }

    #endregion
}

