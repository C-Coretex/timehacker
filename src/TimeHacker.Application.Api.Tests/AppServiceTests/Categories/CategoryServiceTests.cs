using System.Drawing;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.Categories;

public class CategoryServiceTests
{
    #region Mocks

    private readonly Mock<ICategoryRepository> _categoriesRepository = new();
    private readonly Mock<IScheduleEntityRepository> _scheduleEntityRepository = new();

    #endregion

    #region Properties & constructor

    private List<Category> _categories = null!;
    private List<ScheduleEntity> _scheduleEntities = null!;

    private readonly ICategoryAppService _categoryService;
    private readonly Guid _userId = Guid.NewGuid();
    public CategoryServiceTests()
    {
        SetupMocks(_userId);
        _categoryService = new CategoryService(_categoriesRepository.Object, _scheduleEntityRepository.Object);
    }

    #endregion

    [Fact]
    [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
    public async Task AddAsync_ShouldAddEntry()
    {
        var newEntry = new CategoryDto
        {
            Name = "TestCategory1000",
            Description = "",
            Color = Color.AliceBlue
        };
        await _categoryService.AddAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _categories.FirstOrDefault(x => x.Name == newEntry.Name);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should update entry")]
    public async Task UpdateAsync_ShouldUpdateEntry()
    {
        var newEntry = new CategoryDto
        {
            Id = _categories.First(x => x.UserId == _userId).Id,
            Name = "TestCategory1000",
            Description = "",
            Color = Color.AliceBlue
        };
        await _categoryService.UpdateAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should leave the category's schedules untouched")]
    public async Task UpdateAsync_ShouldPreserveSchedules()
    {
        var existing = _categories.First(x => x.UserId == _userId && x.Schedules.Count > 0);
        var scheduleIds = existing.Schedules.Select(x => x.Id).ToList();

        var updateDto = new CategoryDto
        {
            Id = existing.Id,
            Name = "Renamed",
            Description = "",
            Color = Color.AliceBlue
        };

        await _categoryService.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        var result = _categories.First(x => x.Id == existing.Id);
        result.Name.Should().Be("Renamed");
        // Schedules are managed by their own endpoints; saving a category must never drop them.
        result.Schedules.Select(x => x.Id).Should().BeEquivalentTo(scheduleIds);
    }

    [Fact]
    [Trait("DeleteAndSaveAsync", "Should delete entry")]
    public async Task DeleteAsync_ShouldUpdateEntry()
    {
        var idToDelete = _categories.First(x => x.UserId == _userId).Id;
        await _categoryService.DeleteAsync(idToDelete, TestContext.Current.CancellationToken);
        var result = _categories.FirstOrDefault(x => x.Id == idToDelete);
        result.Should().BeNull();
    }

    [Fact]
    [Trait("DeleteAndSaveAsync", "Should delete the recurrences behind the category's schedules")]
    public async Task DeleteAsync_ShouldCascadeScheduleEntitiesOfItsSchedules()
    {
        var existing = _categories.First(x => x.UserId == _userId && x.Schedules.Any(s => s.ScheduleEntityId != null));

        await _categoryService.DeleteAsync(existing.Id, TestContext.Current.CancellationToken);

        // The DB cascades Category -> CategorySchedule, but each window's FK points TO its ScheduleEntity,
        // so those rows would be orphaned unless the service removes them.
        _scheduleEntities.Should().NotContain(x => x.CategorySchedule != null && x.CategorySchedule.CategoryId == existing.Id);
        _categories.Should().NotContain(x => x.Id == existing.Id);
    }

    [Fact]
    [Trait("GetAll", "Should return correct data")]
    public void GetAll_ShouldReturnCorrectData()
    {
        var result = _categoryService.GetAll(TestContext.Current.CancellationToken).ToBlockingEnumerable(TestContext.Current.CancellationToken).ToList();

        // Should only return categories owned by the current user (user-scoped)
        var expectedCount = _categories.Count(c => c.UserId == _userId);
        result.Count.Should().Be(expectedCount);
        result.Should().BeEquivalentTo(_categories.Where(c => c.UserId == _userId).Select(CategoryDto.Create).ToList());
    }

    [Fact]
    [Trait("GetAll", "Should include each category's schedules")]
    public void GetAll_ShouldIncludeSchedules()
    {
        var expected = _categories.First(x => x.UserId == _userId && x.Schedules.Count > 0);

        var result = _categoryService.GetAll(TestContext.Current.CancellationToken)
            .ToBlockingEnumerable(TestContext.Current.CancellationToken)
            .Single(x => x.Id == expected.Id);

        result.Schedules.Should().HaveCount(expected.Schedules.Count);
        result.Schedules.Select(x => x.Description).Should().BeEquivalentTo(expected.Schedules.Select(x => x.Description));
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return correct data")]
    public async Task GetByIdAsync_ShouldUpdateEntry()
    {
        var id = _categories.First(x => x.UserId == _userId).Id;
        var result = await _categoryService.GetByIdAsync(id, TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return every window a category owns on the same day")]
    public async Task GetByIdAsync_ShouldReturnAllSchedulesIncludingSameDayWindows()
    {
        var expected = _categories.First(x => x.UserId == _userId && x.Schedules.Count > 1);

        var result = await _categoryService.GetByIdAsync(expected.Id, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        // Two windows on one date is exactly what a single category could not express before the split.
        result!.Schedules.Should().HaveCount(2);
        result.Schedules.Select(x => x.Date).Distinct().Should().ContainSingle();
        result.Schedules.Select(x => x.Description).Should().BeEquivalentTo(["Working hours", "Overtime"]);
    }

    // Validation Tests
    [Fact]
    [Trait("AddAsync", "Should throw on null input")]
    public async Task AddAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _categoryService.AddAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("UpdateAsync", "Should throw on null input")]
    public async Task UpdateAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _categoryService.UpdateAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return null for non-existent ID")]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNonExistentId()
    {
        var nonExistentId = Guid.NewGuid();

        var result = await _categoryService.GetByIdAsync(nonExistentId, TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    [Trait("DeleteAsync", "Should throw NotFoundException for non-existent ID")]
    public async Task DeleteAsync_ShouldThrowNotFound_ForNonExistentId()
    {
        var nonExistentId = Guid.NewGuid();

        var act = async () => await _categoryService.DeleteAsync(nonExistentId, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // Security Tests
    [Fact]
    [Trait("GetAll", "Should only return user owned categories")]
    public void GetAll_ShouldOnlyReturnUserOwnedCategories()
    {
        var userCategoryIds = _categories.Where(c => c.UserId == _userId).Select(c => c.Id).ToHashSet();

        var result = _categoryService.GetAll(TestContext.Current.CancellationToken).ToBlockingEnumerable(TestContext.Current.CancellationToken).ToList();

        result.Should().NotBeEmpty();
        result.Should().OnlyContain(c => c.Id.HasValue && userCategoryIds.Contains(c.Id.Value));
        result.Count.Should().Be(_categories.Count(c => c.UserId == _userId));
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return null when accessing other user category")]
    public async Task GetByIdAsync_ShouldReturnNull_WhenAccessingOtherUserCategory()
    {
        var otherUserCategory = _categories.First(x => x.UserId != _userId);

        var result = await _categoryService.GetByIdAsync(otherUserCategory.Id, TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    [Trait("UpdateAsync", "Should not update other user categories")]
    public async Task UpdateAsync_ShouldNotUpdateOtherUserCategories()
    {
        var otherUserCategory = _categories.First(x => x.UserId != _userId);
        var originalName = otherUserCategory.Name;

        var updateDto = new CategoryDto
        {
            Id = otherUserCategory.Id,
            Name = "Hacked Name",
            Description = "Hacked Description",
            Color = Color.Red
        };

        // Updating another user's category is rejected (the user-scoped fetch finds nothing).
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _categoryService.UpdateAsync(updateDto, TestContext.Current.CancellationToken));

        var unchangedCategory = _categories.First(x => x.Id == otherUserCategory.Id);
        unchangedCategory.Name.Should().Be(originalName);
        unchangedCategory.Name.Should().NotBe("Hacked Name");
    }

    [Fact]
    [Trait("DeleteAsync", "Should reject deleting other user categories")]
    public async Task DeleteAsync_ShouldRejectOtherUserCategories()
    {
        var otherUserCategory = _categories.First(x => x.UserId != _userId);
        var otherCategoryId = otherUserCategory.Id;

        // Cross-user delete is rejected (not a silent no-op), consistent with UpdateAsync.
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _categoryService.DeleteAsync(otherCategoryId, TestContext.Current.CancellationToken));

        _categories.Should().Contain(x => x.Id == otherCategoryId);
    }

    #region Mock helpers

    private void SetupMocks(Guid userId)
    {
        var withSchedules = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "TestCategory1",
            Color = Color.AliceBlue,
            Description = "Test description"
        };

        // Two windows on one day — the case the Category/CategorySchedule split exists for.
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var recurringWindow = NewSchedule(withSchedules, "Working hours", today, new TimeOnly(09, 00), new TimeOnly(12, 00), Guid.NewGuid());
        var oneOffWindow = NewSchedule(withSchedules, "Overtime", today, new TimeOnly(18, 00), new TimeOnly(20, 00));
        withSchedules.Schedules.Add(recurringWindow);
        withSchedules.Schedules.Add(oneOffWindow);

        _categories =
        [
            withSchedules,

            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "TestCategory2",
                Description = "Test description"
            },

            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "TestCategory3",
                Description = "Test description"
            },

            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "TestCategory4",
                Description = "Test description"
            }
        ];

        _scheduleEntities =
        [
            new()
            {
                Id = recurringWindow.ScheduleEntityId!.Value,
                UserId = userId,
                RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(1)),
                CategorySchedule = recurringWindow
            }
        ];

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
