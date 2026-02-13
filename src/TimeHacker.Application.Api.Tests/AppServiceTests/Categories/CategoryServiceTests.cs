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
        var newEntry = new CategoryDto(null, null, "TestCategory1000", "", Color.AliceBlue);
        await _categoryService.AddAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _categories.FirstOrDefault(x => x.Name == newEntry.Name);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should update entry")]
    public async Task UpdateAsync_ShouldUpdateEntry()
    {
        var newEntry = new CategoryDto(_categories.First(x => x.UserId == _userId).Id, null, "TestCategory1000", "", Color.AliceBlue);
        await _categoryService.UpdateAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
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
    [Trait("DeleteAsync", "Should handle non-existent ID")]
    public async Task DeleteAsync_ShouldHandleNonExistentId()
    {
        var nonExistentId = Guid.NewGuid();

        await _categoryService.DeleteAsync(nonExistentId, TestContext.Current.CancellationToken);
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

        var updateDto = new CategoryDto(
            otherUserCategory.Id,
            null,
            "Hacked Name",
            "Hacked Description",
            Color.Red);

        await _categoryService.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        var unchangedCategory = _categories.First(x => x.Id == otherUserCategory.Id);
        unchangedCategory.Name.Should().Be(originalName);
        unchangedCategory.Name.Should().NotBe("Hacked Name");
    }

    [Fact]
    [Trait("DeleteAsync", "Should not delete other user categories")]
    public async Task DeleteAsync_ShouldNotDeleteOtherUserCategories()
    {
        var otherUserCategory = _categories.First(x => x.UserId != _userId);
        var otherCategoryId = otherUserCategory.Id;

        await _categoryService.DeleteAsync(otherCategoryId, TestContext.Current.CancellationToken);

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
                ScheduleEntity = new ScheduleEntity()
            },

            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "TestCategory2",
                Description = "Test description",
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
