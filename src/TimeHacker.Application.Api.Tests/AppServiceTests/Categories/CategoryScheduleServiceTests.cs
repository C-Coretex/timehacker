using System.Drawing;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.Categories;

public class CategoryScheduleServiceTests
{
    #region Mocks

    private readonly Mock<ICategoryScheduleRepository> _categorySchedulesRepository = new();
    private readonly Mock<ICategoryRepository> _categoriesRepository = new();
    private readonly Mock<IScheduleEntityRepository> _scheduleEntityRepository = new();

    #endregion

    #region Properties & constructor

    private List<CategorySchedule> _schedules = null!;
    private List<Category> _categories = null!;
    private List<ScheduleEntity> _scheduleEntities = null!;

    private Category _category = null!;
    private Category _otherUsersCategory = null!;

    private readonly ICategoryScheduleAppService _service;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.UtcNow);

    public CategoryScheduleServiceTests()
    {
        SetupMocks(_userId);
        _service = new CategoryScheduleAppService(
            _categorySchedulesRepository.Object,
            _categoriesRepository.Object,
            _scheduleEntityRepository.Object);
    }

    #endregion

    [Fact]
    [Trait("AddAsync", "Should add a window to its category")]
    public async Task AddAsync_ShouldAddEntry()
    {
        var newEntry = new CategoryScheduleDto
        {
            CategoryId = _category.Id,
            Description = "Focus block",
            Date = _today,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(12, 00)
        };

        var id = await _service.AddAsync(newEntry, TestContext.Current.CancellationToken);

        var result = _schedules.FirstOrDefault(x => x.Id == id);
        result.Should().NotBeNull();
        result!.Description.Should().Be("Focus block");
        result.CategoryId.Should().Be(_category.Id);
        result.Date.Should().Be(_today);
        result.StartTime.Should().Be(new TimeOnly(09, 00));
        result.EndTime.Should().Be(new TimeOnly(12, 00));
    }

    [Fact]
    [Trait("AddAsync", "Should allow several windows on the same day for one category")]
    public async Task AddAsync_ShouldAllowMultipleWindowsOnTheSameDay()
    {
        var morning = new CategoryScheduleDto
        {
            CategoryId = _category.Id,
            Description = "Working hours",
            Date = _today,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(12, 00)
        };
        var evening = morning with { Description = "Overtime", StartTime = new TimeOnly(18, 00), EndTime = new TimeOnly(20, 00) };

        await _service.AddAsync(morning, TestContext.Current.CancellationToken);
        await _service.AddAsync(evening, TestContext.Current.CancellationToken);

        // Nothing constrains one window per category per day, and overlaps are allowed too.
        _schedules.Where(x => x.CategoryId == _category.Id && x.Date == _today)
            .Select(x => x.Description)
            .Should().Contain(["Working hours", "Overtime"]);
    }

    [Fact]
    [Trait("AddAsync", "Should accept a window with no description")]
    public async Task AddAsync_ShouldAllowNullDescription()
    {
        // The description is only a note distinguishing sibling windows; the category names them.
        var newEntry = new CategoryScheduleDto
        {
            CategoryId = _category.Id,
            Date = _today,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(12, 00)
        };

        var id = await _service.AddAsync(newEntry, TestContext.Current.CancellationToken);

        _schedules.Single(x => x.Id == id).Description.Should().BeNull();
    }

    [Fact]
    [Trait("UpdateAsync", "Should be able to clear the description")]
    public async Task UpdateAsync_ShouldAllowClearingDescription()
    {
        var existing = _schedules.First(x => x.UserId == _userId && x.Description != null);

        var updateDto = new CategoryScheduleDto
        {
            Id = existing.Id,
            CategoryId = existing.CategoryId,
            Description = null,
            Date = existing.Date,
            StartTime = existing.StartTime,
            EndTime = existing.EndTime
        };

        await _service.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        _schedules.First(x => x.Id == existing.Id).Description.Should().BeNull();
    }

    [Fact]
    [Trait("AddAsync", "Should throw on null input")]
    public async Task AddAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _service.AddAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("AddAsync", "Should throw NotFoundException when the parent category does not exist")]
    public async Task AddAsync_ShouldThrowNotFound_WhenCategoryMissing()
    {
        var newEntry = new CategoryScheduleDto
        {
            CategoryId = Guid.NewGuid(),
            Description = "Orphan",
            Date = _today,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(12, 00)
        };

        var act = async () => await _service.AddAsync(newEntry, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<NotFoundException>();
        _schedules.Should().NotContain(x => x.Description == "Orphan");
    }

    [Fact]
    [Trait("AddAsync", "Should throw NotFoundException for another user's category")]
    public async Task AddAsync_ShouldThrowNotFound_ForAnotherUsersCategory()
    {
        var newEntry = new CategoryScheduleDto
        {
            CategoryId = _otherUsersCategory.Id,
            Description = "Intruder",
            Date = _today,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(12, 00)
        };

        // RLS makes the other user's category invisible, so it is indistinguishable from a missing one.
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.AddAsync(newEntry, TestContext.Current.CancellationToken));

        _schedules.Should().NotContain(x => x.Description == "Intruder");
    }

    [Fact]
    [Trait("UpdateAsync", "Should update the window")]
    public async Task UpdateAsync_ShouldUpdateEntry()
    {
        var existing = _schedules.First(x => x.UserId == _userId);

        var updateDto = new CategoryScheduleDto
        {
            Id = existing.Id,
            CategoryId = existing.CategoryId,
            Description = "Renamed window",
            Date = _today.AddDays(3),
            StartTime = new TimeOnly(07, 30),
            EndTime = new TimeOnly(12, 45)
        };

        await _service.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        var result = _schedules.First(x => x.Id == existing.Id);
        result.Description.Should().Be("Renamed window");
        result.Date.Should().Be(_today.AddDays(3));
        result.StartTime.Should().Be(new TimeOnly(07, 30));
        result.EndTime.Should().Be(new TimeOnly(12, 45));
    }

    [Fact]
    [Trait("UpdateAsync", "Should keep the attached recurrence")]
    public async Task UpdateAsync_ShouldPreserveScheduleEntityId()
    {
        var existing = _schedules.First(x => x.UserId == _userId && x.ScheduleEntityId != null);
        var scheduleEntityId = existing.ScheduleEntityId;

        var updateDto = new CategoryScheduleDto
        {
            Id = existing.Id,
            CategoryId = existing.CategoryId,
            Description = "Renamed",
            Date = existing.Date,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(18, 00)
        };

        await _service.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        // The recurrence link is owned by ScheduleEntityAppService; a window edit must not clear it.
        var result = _schedules.First(x => x.Id == existing.Id);
        result.Description.Should().Be("Renamed");
        result.ScheduleEntityId.Should().Be(scheduleEntityId);
    }

    [Fact]
    [Trait("UpdateAsync", "Should throw on null input")]
    public async Task UpdateAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _service.UpdateAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("UpdateAsync", "Should throw NotFoundException for non-existent ID")]
    public async Task UpdateAsync_ShouldThrowNotFound_ForNonExistentId()
    {
        var updateDto = new CategoryScheduleDto
        {
            Id = Guid.NewGuid(),
            CategoryId = _category.Id,
            Description = "Ghost",
            Date = _today,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(18, 00)
        };

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateAsync(updateDto, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("UpdateAsync", "Should not update another user's window")]
    public async Task UpdateAsync_ShouldRejectAnotherUsersSchedule()
    {
        var otherUsersSchedule = _schedules.First(x => x.UserId != _userId);
        var originalDescription = otherUsersSchedule.Description;

        var updateDto = new CategoryScheduleDto
        {
            Id = otherUsersSchedule.Id,
            CategoryId = otherUsersSchedule.CategoryId,
            Description = "Hacked window",
            Date = _today,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(18, 00)
        };

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateAsync(updateDto, TestContext.Current.CancellationToken));

        _schedules.First(x => x.Id == otherUsersSchedule.Id).Description.Should().Be(originalDescription);
    }

    [Fact]
    [Trait("DeleteAsync", "Should delete the window")]
    public async Task DeleteAsync_ShouldDeleteEntry()
    {
        var existing = _schedules.First(x => x.UserId == _userId && x.ScheduleEntityId == null);

        await _service.DeleteAsync(existing.CategoryId, existing.Id, TestContext.Current.CancellationToken);

        _schedules.Should().NotContain(x => x.Id == existing.Id);
    }

    [Fact]
    [Trait("DeleteAsync", "Should cascade the attached recurrence")]
    public async Task DeleteAsync_ShouldCascadeScheduleEntity()
    {
        var existing = _schedules.First(x => x.UserId == _userId && x.ScheduleEntityId != null);
        var scheduleEntityId = existing.ScheduleEntityId!.Value;

        await _service.DeleteAsync(existing.CategoryId, existing.Id, TestContext.Current.CancellationToken);

        // The window's FK points TO its ScheduleEntity, so the recurrence must go with it.
        _scheduleEntities.Should().NotContain(x => x.Id == scheduleEntityId);
    }

    [Fact]
    [Trait("DeleteAsync", "Should throw NotFoundException for non-existent ID")]
    public async Task DeleteAsync_ShouldThrowNotFound_ForNonExistentId()
    {
        var act = async () => await _service.DeleteAsync(_category.Id, Guid.NewGuid(), TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    [Trait("DeleteAsync", "Should throw NotFoundException when the window belongs to another category")]
    public async Task DeleteAsync_ShouldThrowNotFound_WhenCategoryDoesNotOwnTheSchedule()
    {
        var existing = _schedules.First(x => x.UserId == _userId);

        // The route's categoryId is part of the predicate, so a window cannot be deleted through
        // a category that does not own it.
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.DeleteAsync(Guid.NewGuid(), existing.Id, TestContext.Current.CancellationToken));

        _schedules.Should().Contain(x => x.Id == existing.Id);
    }

    [Fact]
    [Trait("DeleteAsync", "Should reject deleting another user's window")]
    public async Task DeleteAsync_ShouldRejectAnotherUsersSchedule()
    {
        var otherUsersSchedule = _schedules.First(x => x.UserId != _userId);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.DeleteAsync(otherUsersSchedule.CategoryId, otherUsersSchedule.Id, TestContext.Current.CancellationToken));

        _schedules.Should().Contain(x => x.Id == otherUsersSchedule.Id);
    }

    [Fact]
    [Trait("GetAllForCategory", "Should return only that category's windows, ordered")]
    public void GetAllForCategory_ShouldReturnOnlyItsOwnSchedules()
    {
        var result = _service.GetAllForCategory(_category.Id, TestContext.Current.CancellationToken)
            .ToBlockingEnumerable(TestContext.Current.CancellationToken)
            .ToList();

        result.Should().OnlyContain(x => x.CategoryId == _category.Id);
        result.Should().HaveCount(_schedules.Count(x => x.CategoryId == _category.Id && x.UserId == _userId));
        result.Select(x => (x.Date, x.StartTime)).Should().BeInAscendingOrder();
    }

    [Fact]
    [Trait("GetAllForCategory", "Should not return another user's windows")]
    public void GetAllForCategory_ShouldNotReturnAnotherUsersSchedules()
    {
        var result = _service.GetAllForCategory(_otherUsersCategory.Id, TestContext.Current.CancellationToken)
            .ToBlockingEnumerable(TestContext.Current.CancellationToken)
            .ToList();

        result.Should().BeEmpty();
    }

    #region Mock helpers

    private void SetupMocks(Guid userId)
    {
        var otherUserId = Guid.NewGuid();

        _category = new Category { Id = Guid.NewGuid(), UserId = userId, Name = "Work", Color = Color.SteelBlue };
        _otherUsersCategory = new Category { Id = Guid.NewGuid(), UserId = otherUserId, Name = "Someone else's", Color = Color.Red };
        _categories = [_category, _otherUsersCategory];

        var recurringScheduleEntityId = Guid.NewGuid();

        _schedules =
        [
            NewSchedule(_category, "Working hours", _today, new TimeOnly(09, 00), new TimeOnly(12, 00), recurringScheduleEntityId),
            NewSchedule(_category, "Overtime", _today, new TimeOnly(18, 00), new TimeOnly(20, 00)),
            NewSchedule(_otherUsersCategory, "Their window", _today, new TimeOnly(10, 00), new TimeOnly(11, 00))
        ];

        _scheduleEntities =
        [
            new()
            {
                Id = recurringScheduleEntityId,
                UserId = userId,
                RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(1)),
                CategorySchedule = _schedules[0]
            }
        ];

        _categorySchedulesRepository.As<IUserScopedRepositoryBase<CategorySchedule, Guid>>().SetupRepositoryMock(_schedules, userId);
        _categoriesRepository.As<IUserScopedRepositoryBase<Category, Guid>>().SetupRepositoryMock(_categories, userId);
        _scheduleEntityRepository.As<IUserScopedRepositoryBase<ScheduleEntity, Guid>>().SetupRepositoryMock(_scheduleEntities, userId);
    }

    private static CategorySchedule NewSchedule(Category category, string? description, DateOnly date, TimeOnly start, TimeOnly end, Guid? scheduleEntityId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = category.UserId,
            CategoryId = category.Id,
            Category = category,
            Description = description,
            Date = date,
            StartTime = start,
            EndTime = end,
            ScheduleEntityId = scheduleEntityId
        };

    #endregion
}
