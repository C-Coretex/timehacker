using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.Tasks;

public class DynamicTaskAppServiceTests
{
    #region Mocks

    private readonly Mock<IDynamicTaskRepository> _dynamicTasksRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();

    #endregion

    #region Properties & constructor

    private List<DynamicTask> _dynamicTasks = null!;
    private List<Category> _categories = null!;

    private readonly IDynamicTaskAppService _dynamicTaskAppService;
    private readonly Guid _userId = Guid.NewGuid();

    private Guid OwnCategoryId => _categories.First(x => x.UserId == _userId).Id;

    public DynamicTaskAppServiceTests()
    {
        SetupMocks(_userId);
        _dynamicTaskAppService = new DynamicTaskAppService(_dynamicTasksRepository.Object, _categoryRepository.Object);
    }

    #endregion

    [Fact]
    [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
    public async Task AddAsync_ShouldAddEntry()
    {
        var newEntry = new DynamicTaskDto()
        {
            Name = "TestDynamicTask1000",
            Categories = [LinkCategoryDto.EmptyLink(OwnCategoryId)]
        };
        await _dynamicTaskAppService.AddAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _dynamicTasks.FirstOrDefault(x => x.Name == newEntry.Name);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should update entry")]
    public async Task UpdateAsync_ShouldUpdateEntry()
    {
        var newEntry = new DynamicTaskDto()
        {
            Id = _dynamicTasks.First(x => x.UserId == _userId).Id,
            Name = "TestDynamicTask1000",
            Categories = [LinkCategoryDto.EmptyLink(OwnCategoryId)]
        };
        await _dynamicTaskAppService.UpdateAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _dynamicTasks.FirstOrDefault(x => x.Id == newEntry.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAsync", "Should not double update entity")]
    public async Task UpdateAsync_ShouldNotDoubleUpdateEntity()
    {
        var taskToUpdate = _dynamicTasks.First(x => x.UserId == _userId);
        var originalName = taskToUpdate.Name;

        var updateDto = new DynamicTaskDto
        {
            Id = taskToUpdate.Id,
            Name = "Updated Name Once",
            Priority = 3,
            Categories = [LinkCategoryDto.EmptyLink(OwnCategoryId)]
        };

        await _dynamicTaskAppService.UpdateAsync(updateDto, TestContext.Current.CancellationToken);

        var updatedTask = _dynamicTasks.First(x => x.Id == taskToUpdate.Id);
        updatedTask.Name.Should().Be("Updated Name Once");
        updatedTask.Priority.Should().Be(3);

        // Verify the task list doesn't have duplicates or corruption from double update
        _dynamicTasks.Count(x => x.Id == taskToUpdate.Id).Should().Be(1);
    }

    [Fact]
    [Trait("DeleteAndSaveAsync", "Should delete entry")]
    public async Task DeleteAsync_ShouldDeleteEntry()
    {
        var id = _dynamicTasks.First(x => x.UserId == _userId).Id;
        await _dynamicTaskAppService.DeleteAsync(id, TestContext.Current.CancellationToken);
        var result = _dynamicTasks.FirstOrDefault(x => x.Id == id);
        result.Should().BeNull();
    }

    [Fact]
    [Trait("GetAll", "Should return correct data")]
    public async Task GetAll_ShouldReturnCorrectData()
    {
        var result = await _dynamicTaskAppService.GetAll(TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        result.Count.Should().Be(_dynamicTasks.Count);
        result.Should().BeEquivalentTo(_dynamicTasks.Select(DynamicTaskDto.Create).ToList());
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return correct data")]
    public async Task GetByIdAsync_ShouldUpdateEntry()
    {
        var id = _dynamicTasks.First(x => x.UserId == _userId).Id;
        var result = await _dynamicTaskAppService.GetByIdAsync(id, TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    [Trait("AddAsync", "Should link the given categories")]
    public async Task AddAsync_ShouldLinkCategories()
    {
        var categoryId = OwnCategoryId;

        await _dynamicTaskAppService.AddAsync(new DynamicTaskDto
        {
            Name = "Categorised",
            Categories = [LinkCategoryDto.EmptyLink(categoryId)]
        }, TestContext.Current.CancellationToken);

        _dynamicTasks.Single(x => x.Name == "Categorised")
            .CategoryDynamicTasks.Should().ContainSingle(link => link.CategoryId == categoryId);
    }

    [Fact]
    [Trait("AddAsync", "Should collapse a repeated category id")]
    public async Task AddAsync_ShouldNotLinkTheSameCategoryTwice()
    {
        var categoryId = OwnCategoryId;

        await _dynamicTaskAppService.AddAsync(new DynamicTaskDto
        {
            Name = "Categorised",
            Categories = [LinkCategoryDto.EmptyLink(categoryId), LinkCategoryDto.EmptyLink(categoryId)]
        }, TestContext.Current.CancellationToken);

        _dynamicTasks.Single(x => x.Name == "Categorised").CategoryDynamicTasks.Should().HaveCount(1);
    }

    [Fact]
    [Trait("UpdateAsync", "Should replace the linked categories")]
    public async Task UpdateAsync_ShouldReplaceCategories()
    {
        var task = _dynamicTasks.First(x => x.UserId == _userId);
        var categoryId = OwnCategoryId;
        task.CategoryDynamicTasks.Add(new CategoryDynamicTask { CategoryId = Guid.NewGuid(), DynamicTaskId = task.Id });

        await _dynamicTaskAppService.UpdateAsync(new DynamicTaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Categories = [LinkCategoryDto.EmptyLink(categoryId)]
        }, TestContext.Current.CancellationToken);

        task.CategoryDynamicTasks.Should().ContainSingle(link => link.CategoryId == categoryId);
    }

    [Fact]
    [Trait("AddAsync+UpdateAsync", "Should reject a dynamic task with no category")]
    public async Task AddAsync_And_UpdateAsync_ShouldThrow_WhenNoCategoriesGiven()
    {
        // Unlike a fixed task, a dynamic one must always carry at least one category.
        var task = _dynamicTasks.First(x => x.UserId == _userId);
        task.CategoryDynamicTasks.Add(new CategoryDynamicTask { CategoryId = OwnCategoryId, DynamicTaskId = task.Id });

        var add = async () => await _dynamicTaskAppService.AddAsync(
            new DynamicTaskDto { Name = "Uncategorised" }, TestContext.Current.CancellationToken);
        await add.Should().ThrowAsync<DataIsNotCorrectException>();

        var update = async () => await _dynamicTaskAppService.UpdateAsync(
            new DynamicTaskDto { Id = task.Id, Name = task.Name }, TestContext.Current.CancellationToken);
        await update.Should().ThrowAsync<DataIsNotCorrectException>();

        // The rejected update left the existing link alone.
        _dynamicTasks.Should().NotContain(x => x.Name == "Uncategorised");
        task.CategoryDynamicTasks.Should().ContainSingle();
    }

    [Theory]
    [Trait("AddAsync+UpdateAsync", "Should reject a category the user cannot see")]
    [InlineData(true), InlineData(false)]
    public async Task AddAsync_And_UpdateAsync_ShouldThrowNotFound_ForInvisibleCategory(bool anotherUsersCategory)
    {
        // A foreign key check bypasses RLS, so an unguarded link would be written and then read back as nothing.
        var categoryId = anotherUsersCategory
            ? _categories.First(x => x.UserId != _userId).Id
            : Guid.NewGuid();
        var categories = new[] { LinkCategoryDto.EmptyLink(categoryId) };

        var add = async () => await _dynamicTaskAppService.AddAsync(
            new DynamicTaskDto { Name = "Sneaky", Categories = categories }, TestContext.Current.CancellationToken);
        (await add.Should().ThrowAsync<NotFoundException>()).Which.ResourceName.Should().Be("Category");

        var update = async () => await _dynamicTaskAppService.UpdateAsync(
            new DynamicTaskDto { Id = _dynamicTasks.First(x => x.UserId == _userId).Id, Name = "Sneaky", Categories = categories },
            TestContext.Current.CancellationToken);
        (await update.Should().ThrowAsync<NotFoundException>()).Which.ResourceName.Should().Be("Category");

        _dynamicTasks.Should().NotContain(x => x.Name == "Sneaky");
    }

    // Validation Tests
    [Fact]
    [Trait("AddAsync", "Should throw on null input")]
    public async Task AddAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _dynamicTaskAppService.AddAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("UpdateAsync", "Should throw on null input")]
    public async Task UpdateAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _dynamicTaskAppService.UpdateAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return null for non-existent ID")]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNonExistentId()
    {
        var result = await _dynamicTaskAppService.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);
        result.Should().BeNull();
    }

    #region Mock helpers

    private void SetupMocks(Guid userId)
    {
        _dynamicTasks =
        [
            new()
            {
                UserId = userId,
                Name = "TestDynamicTask1",
                Priority = 1,
                Description = "Test description",
                MinTimeToFinish = new TimeSpan(0, 30, 0),
                MaxTimeToFinish = new TimeSpan(1, 0, 0),
                OptimalTimeToFinish = new TimeSpan(0, 45, 0)
            },

            new()
            {
                UserId = userId,
                Name = "TestDynamicTask2",
                Priority = 1,
                Description = "Test description",
                MinTimeToFinish = new TimeSpan(0, 30, 0),
                MaxTimeToFinish = new TimeSpan(1, 0, 0),
                OptimalTimeToFinish = new TimeSpan(0, 45, 0)
            },

            new()
            {
                UserId = Guid.NewGuid(),
                Name = "TestDynamicTask3",
                Priority = 1,
                Description = "Test description",
                MinTimeToFinish = new TimeSpan(0, 30, 0),
                MaxTimeToFinish = new TimeSpan(1, 0, 0),
                OptimalTimeToFinish = new TimeSpan(0, 45, 0)
            }
        ];

        _dynamicTasksRepository.As<IUserScopedRepositoryBase<DynamicTask, Guid>>().SetupRepositoryMock(_dynamicTasks);

        _categories =
        [
            new() { UserId = userId, Name = "Own category" },
            new() { UserId = Guid.NewGuid(), Name = "Another user's category" }
        ];
        _categoryRepository.As<IUserScopedRepositoryBase<Category, Guid>>().SetupRepositoryMock(_categories, userId);
    }

    #endregion
}
