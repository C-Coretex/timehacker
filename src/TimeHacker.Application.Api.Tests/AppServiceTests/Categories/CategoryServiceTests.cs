using System.Drawing;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.Categories;

public class CategoryServiceTests
{
    #region Mocks

    private readonly Mock<ICategoryRepository> _categoriesRepository = new();

    #endregion

    #region Properties & constructor

    private List<Category> _categories = null!;

    private readonly ICategoryAppService _categoryService;
    private readonly Guid _userId = Guid.NewGuid();
    public CategoryServiceTests()
    {
        SetupMocks(_userId);
        _categoryService = new CategoryService(_categoriesRepository.Object);
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
            Color = Color.AliceBlue,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(18, 00)
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
            Color = Color.AliceBlue,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(18, 00)
        };
        await _categoryService.UpdateAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should keep the attached schedule")]
    public async Task UpdateAsync_ShouldPreserveScheduleEntityId()
    {
        var existing = _categories.First(x => x.UserId == _userId && x.ScheduleEntityId != null);
        var scheduleEntityId = existing.ScheduleEntityId;

        var updateDto = new CategoryDto
        {
            Id = existing.Id,
            Name = "Renamed",
            Description = "",
            Color = Color.AliceBlue,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(18, 00)
        };

        await _categoryService.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        var result = _categories.First(x => x.Id == existing.Id);
        result.Name.Should().Be("Renamed");
        result.ScheduleEntityId.Should().Be(scheduleEntityId);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should update the time window")]
    public async Task UpdateAsync_ShouldUpdateTimeWindow()
    {
        var existing = _categories.First(x => x.UserId == _userId);

        var updateDto = new CategoryDto
        {
            Id = existing.Id,
            Name = existing.Name,
            Description = existing.Description,
            Color = existing.Color,
            StartTime = new TimeOnly(07, 30),
            EndTime = new TimeOnly(12, 45)
        };

        await _categoryService.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        var result = _categories.First(x => x.Id == existing.Id);
        result.StartTime.Should().Be(new TimeOnly(07, 30));
        result.EndTime.Should().Be(new TimeOnly(12, 45));
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
    [Trait("GetByIdAsync", "Should return correct data")]
    public async Task GetByIdAsync_ShouldUpdateEntry()
    {
        var id = _categories.First(x => x.UserId == _userId).Id;
        var result = await _categoryService.GetByIdAsync(id, TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
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
    public async Task GetAll_ShouldOnlyReturnUserOwnedCategories()
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
            Color = Color.Red,
            StartTime = new TimeOnly(09, 00),
            EndTime = new TimeOnly(18, 00)
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
        _categories =
        [
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "TestCategory1",
                Color = Color.AliceBlue,
                Description = "Test description",
                StartTime = new TimeOnly(09, 00),
                EndTime = new TimeOnly(18, 00),
                ScheduleEntityId = Guid.NewGuid(),
                ScheduleEntity = new ScheduleEntity()
            },

            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "TestCategory2",
                Description = "Test description",
                StartTime = new TimeOnly(12, 00),
                EndTime = new TimeOnly(14, 00),
            },

            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "TestCategory3",
                Description = "Test description",
                ScheduleEntity = new ScheduleEntity()
            },

            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "TestCategory4",
                Description = "Test description",
            }
        ];

        _categoriesRepository.As<IUserScopedRepositoryBase<Category, Guid>>().SetupRepositoryMock(_categories, userId);
    }

    #endregion
}
