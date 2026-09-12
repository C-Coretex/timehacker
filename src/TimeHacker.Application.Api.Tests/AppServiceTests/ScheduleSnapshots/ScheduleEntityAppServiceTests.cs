using Microsoft.Extensions.Time.Testing;
using System.Drawing;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.ScheduleSnapshots;

public class ScheduleEntityAppServiceTests
{        
    #region Mocks

    private readonly Mock<IScheduleEntityRepository> _scheduleEntityRepository = new();
    private readonly Mock<IFixedTaskRepository> _fixedTasksRepository = new();
    private readonly Mock<ICategoryScheduleRepository> _categorySchedulesRepository = new();

    #endregion

    #region Properties & constructor

    private List<FixedTask> _fixedTasks = null!;
    private List<CategorySchedule> _categorySchedules = null!;
    private List<ScheduleEntity> _scheduledEntities = null!;

    private readonly Guid _userId = Guid.NewGuid();

    private readonly IScheduleEntityAppService _scheduleEntityAppService;

    public ScheduleEntityAppServiceTests()
    {
        SetupMocks(_userId);
        _scheduleEntityAppService = new ScheduleEntityAppService(_scheduleEntityRepository.Object, _fixedTasksRepository.Object, _categorySchedulesRepository.Object, TimeProvider.System);
    }

    #endregion

    [Theory, CombinatorialData]
    [Trait("Save", "Should save data")]
    public async Task Save_ShouldSaveData(bool isCategory)
    {
        var repeatingEntity = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));

        var inputData = new ScheduleEntityCreateDto(
            isCategory ? ScheduleEntityParentType.CategorySchedule : ScheduleEntityParentType.FixedTask,
            isCategory
                ? _categorySchedules.First(x => x.UserId == _userId).Id
                : _fixedTasks.First(x => x.UserId == _userId).Id,
            repeatingEntity
            );
        var expected = await _scheduleEntityAppService.Save(inputData, TestContext.Current.CancellationToken);

        var actual2 = isCategory
            ? _categorySchedules.First(x => x.Id == inputData.ParentEntityId).ScheduleEntityId
            : _fixedTasks.First(x => x.Id == inputData.ParentEntityId).ScheduleEntityId;

        expected.Id.Should().Be(actual2!.Value);
        var scheduledEntity = _scheduledEntities.First(x => x.Id == expected.Id);
        scheduledEntity.Should().NotBeNull();
        scheduledEntity.RepeatingEntity.Should().Be(repeatingEntity);
    }


    [Theory, CombinatorialData]
    [Trait("Save", "Should throw exception on incorrect data")]
    public async Task Save_ShouldThrow(bool existingEntry, bool isCategory)
    {
        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            var actual = await _scheduleEntityAppService.Save(new ScheduleEntityCreateDto(
                isCategory ? ScheduleEntityParentType.CategorySchedule : ScheduleEntityParentType.FixedTask,
                existingEntry ? _scheduledEntities.First(x => x.UserId != _userId).Id : Guid.NewGuid(),
                new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(1))), TestContext.Current.CancellationToken);
        });
    }

    [Fact]
    [Trait("Save", "Should throw on null repeating entity")]
    public async Task Save_ShouldThrowOnNullRepeatingEntity()
    {
        var fixedTask = _fixedTasks.First(x => x.UserId == _userId);
        var createDto = new ScheduleEntityCreateDto(
            ScheduleEntityParentType.FixedTask,
            fixedTask.Id,
            null!); // Null RepeatingEntity

        await Assert.ThrowsAnyAsync<Exception>(() =>
            _scheduleEntityAppService.Save(createDto, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("Save", "Should respect user scoping")]
    public async Task Save_ShouldRespectUserScoping()
    {
        var otherUserTask = _fixedTasks.First(x => x.UserId != _userId);
        var createDto = new ScheduleEntityCreateDto(
            ScheduleEntityParentType.FixedTask,
            otherUserTask.Id,
            new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(1)));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _scheduleEntityAppService.Save(createDto, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("Save", "Should update schedule entity ID on parent")]
    public async Task Save_ShouldUpdateScheduleEntityIdOnParent()
    {
        var fixedTask = _fixedTasks.First(x => x.UserId == _userId && x.ScheduleEntityId == null);
        var originalScheduleEntityId = fixedTask.ScheduleEntityId;

        var createDto = new ScheduleEntityCreateDto(
            ScheduleEntityParentType.FixedTask,
            fixedTask.Id,
            new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(1)));

        var result = await _scheduleEntityAppService.Save(createDto, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        var updatedTask = _fixedTasks.First(x => x.Id == fixedTask.Id);
        updatedTask.ScheduleEntityId.Should().NotBeNull();
        updatedTask.ScheduleEntityId.Should().Be(result.Id);
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
                RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2)),
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

        _scheduleEntityRepository.As<IUserScopedRepositoryBase<ScheduleEntity, Guid>>().SetupRepositoryMock(_scheduledEntities, userId);


        _fixedTasks =
        [
            new()
            {
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
                UserId = userId,
                Name = "TestFixedTask2",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = DateTime.Now.AddHours(2),
                EndTimestamp = DateTime.Now.AddHours(2).AddMinutes(30)
            },

            new()
            {
                UserId = Guid.NewGuid(),
                Name = "TestFixedTask3",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = DateTime.Now.AddHours(3),
                EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30),
                ScheduleEntity = new ScheduleEntity()
            },

            new()
            {
                UserId = Guid.NewGuid(),
                Name = "TestFixedTask4",
                Priority = 1,
                Description = "Test description",
                StartTimestamp = DateTime.Now.AddDays(-2).AddHours(3),
                EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30)
            }
        ];

        _fixedTasksRepository.As<IUserScopedRepositoryBase<FixedTask, Guid>>().SetupRepositoryMock(_fixedTasks, userId);

        _fixedTasksRepository.Setup(x => x.UpdateProperty(It.IsAny<Expression<Func<FixedTask, bool>>>(), It.IsAny<Expression<Func<FixedTask, Guid?>>>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<FixedTask, bool>>, Expression<Func<FixedTask, Guid?>>, Guid?, CancellationToken>((predicate, _, value, _) =>
            {   
                var items = _fixedTasks.Where(predicate.Compile());
                foreach (var item in items)
                    item.ScheduleEntityId = value;
            });

        // A recurrence anchors on its window's own Date, so these are relative to today — a hardcoded
        // date would eventually fall behind the "Once" floor and change what these tests exercise.
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var category = new Category { UserId = userId, Name = "TestCategory1", Color = Color.AliceBlue };
        var otherUsersCategory = new Category { UserId = Guid.NewGuid(), Name = "TestCategory3" };

        _categorySchedules =
        [
            NewCategorySchedule(category, "Working hours", today, new ScheduleEntity()),
            NewCategorySchedule(category, "Overtime", today),
            NewCategorySchedule(otherUsersCategory, "Their window", today, new ScheduleEntity()),
            NewCategorySchedule(otherUsersCategory, "Their other window", today)
        ];

        _categorySchedulesRepository.As<IUserScopedRepositoryBase<CategorySchedule, Guid>>().SetupRepositoryMock(_categorySchedules, userId);
        _categorySchedulesRepository.Setup(x => x.UpdateProperty(It.IsAny<Expression<Func<CategorySchedule, bool>>>(), It.IsAny<Expression<Func<CategorySchedule, Guid?>>>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<CategorySchedule, bool>>, Expression<Func<CategorySchedule, Guid?>>, Guid?, CancellationToken>((predicate, _, value, _) =>
            {
                var items = _categorySchedules.Where(predicate.Compile());
                foreach (var item in items)
                    item.ScheduleEntityId = value;
            });
    }

    private static CategorySchedule NewCategorySchedule(Category category, string? description, DateOnly date, ScheduleEntity? scheduleEntity = null) =>
        new()
        {
            UserId = category.UserId,
            CategoryId = category.Id,
            Category = category,
            Description = description,
            Date = date,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(18, 00),
            ScheduleEntity = scheduleEntity
        };

    #endregion
}
