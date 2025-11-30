using System.Drawing;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.Categories;

public class CategoryServiceTests
{
    #region Mocks

    private readonly Mock<ICategoryRepository> _categoriesRepository = new();

    #endregion

    #region Properties & constructor

    private List<Category> _categories;

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
        await _categoryService.AddAsync(newEntry);
        var result = _categories.FirstOrDefault(x => x.Name == newEntry.Name);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should update entry")]
    public async Task UpdateAsync_ShouldUpdateEntry()
    {
        var newEntry = new CategoryDto(_categories.First(x => x.UserId == _userId).Id, null, "TestCategory1000", "", Color.AliceBlue);
        await _categoryService.UpdateAsync(newEntry);
        var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("DeleteAndSaveAsync", "Should delete entry")]
    public async Task DeleteAsync_ShouldUpdateEntry()
    {
        var idToDelete = _categories.First(x => x.UserId == _userId).Id;
        await _categoryService.DeleteAsync(idToDelete);
        var result = _categories.FirstOrDefault(x => x.Id == idToDelete);
        result.Should().BeNull();
    }

    [Fact]
    [Trait("GetAll", "Should return correct data")]
    public void GetAll_ShouldReturnCorrectData()
    {
        var result = _categoryService.GetAll().ToBlockingEnumerable().ToList();

        result.Count.Should().Be(_categories.Count);
        result.Should().BeEquivalentTo(_categories.Select(CategoryDto.Create).ToList());
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return correct data")]
    public async Task GetByIdAsync_ShouldUpdateEntry()
    {
        var id = _categories.First(x => x.UserId == _userId).Id;
        var result = await _categoryService.GetByIdAsync(id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
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

        _categoriesRepository.As<IUserScopedRepositoryBase<Category, Guid>>().SetupRepositoryMock(_categories);
    }

    #endregion
}
