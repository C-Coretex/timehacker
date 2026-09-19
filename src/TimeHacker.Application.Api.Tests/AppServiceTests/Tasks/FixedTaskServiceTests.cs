namespace TimeHacker.Application.Api.Tests.AppServiceTests.Tasks;

public class FixedTaskAppServiceTests
{
    #region Mocks

    private readonly Mock<IFixedTaskRepository> _fixedTasksRepository = new();
    private readonly Mock<IScheduleEntityRepository> _scheduleEntityRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();

    #endregion

    #region Properties & constructor

    private List<FixedTask> _fixedTasks = null!;
    private List<ScheduleEntity> _scheduleEntities = null!;
    private List<Category> _categories = null!;

    private readonly IFixedTaskAppService _fixedTaskAppService;
    private readonly Guid _userId = Guid.NewGuid();

    private Guid OwnCategoryId => _categories.First(x => x.UserId == _userId).Id;

    public FixedTaskAppServiceTests()
    {
        SetupMocks(_userId);
        _fixedTaskAppService = new FixedTaskAppService(_fixedTasksRepository.Object, _scheduleEntityRepository.Object, _categoryRepository.Object);
    }

    #endregion

    [Fact]
    [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
    public async Task AddAsync_ShouldAddEntry()
    {
        var newEntry = new FixedTaskDto()
        {
            Name = "TestFixedTask1000"
        };
        await _fixedTaskAppService.AddAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _fixedTasks.FirstOrDefault(x => x.Name == newEntry.Name);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("UpdateAndSaveAsync", "Should update entry")]
    public async Task UpdateAsync_ShouldUpdateEntry()
    {
        var newEntry = new FixedTaskDto()
        {
            Id = _fixedTasks.First(x => x.UserId == _userId).Id,
            Name = "TestFixedTask1000"
        };
        await _fixedTaskAppService.UpdateAsync(newEntry, TestContext.Current.CancellationToken);
        var result = _fixedTasks.FirstOrDefault(x => x.Id == newEntry.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be(newEntry.Name);
    }

    [Fact]
    [Trait("DeleteAndSaveAsync", "Should delete entry")]
    public async Task DeleteAsync_ShouldUpdateEntry()
    {
        var idToDelete = _fixedTasks.First(x => x.UserId == _userId).Id;
        await _fixedTaskAppService.DeleteAsync(idToDelete, TestContext.Current.CancellationToken);
        var result = _fixedTasks.FirstOrDefault(x => x.Id == idToDelete);
        result.Should().BeNull();
    }

    [Fact]
    [Trait("DeleteAsync", "Should cascade delete schedule entities")]
    public async Task DeleteAsync_ShouldCascadeDeleteScheduleEntities()
    {
        var fixedTaskId = Guid.NewGuid();
        var fixedTask = new FixedTask
        {
            Id = fixedTaskId,
            UserId = _userId,
            Name = "Task with Schedule",
            Priority = 1,
            StartTimestamp = DateTime.Now,
            EndTimestamp = DateTime.Now.AddHours(1)
        };
        _fixedTasks.Add(fixedTask);

        var scheduleEntity = new ScheduleEntity
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            FixedTask = fixedTask,
            CreatedTimestamp = DateTime.Now
        };
        _scheduleEntities.Add(scheduleEntity);

        await _fixedTaskAppService.DeleteAsync(fixedTaskId, TestContext.Current.CancellationToken);

        _fixedTasks.Should().NotContain(x => x.Id == fixedTaskId);

        _scheduleEntities.Should().NotContain(x => x.FixedTask != null && x.FixedTask.Id == fixedTaskId);
    }

    [Fact]
    [Trait("DeleteAsync", "Should not delete unrelated schedule entities")]
    public async Task DeleteAsync_ShouldNotDeleteUnrelatedScheduleEntities()
    {
        var fixedTask1Id = Guid.NewGuid();
        var fixedTask1 = new FixedTask
        {
            Id = fixedTask1Id,
            UserId = _userId,
            Name = "Task 1",
            Priority = 1,
            StartTimestamp = DateTime.Now,
            EndTimestamp = DateTime.Now.AddHours(1)
        };
        var fixedTask2Id = Guid.NewGuid();
        var fixedTask2 = new FixedTask
        {
            Id = fixedTask2Id,
            UserId = _userId,
            Name = "Task 2",
            Priority = 1,
            StartTimestamp = DateTime.Now,
            EndTimestamp = DateTime.Now.AddHours(1)
        };
        _fixedTasks.Add(fixedTask1);
        _fixedTasks.Add(fixedTask2);

        var scheduleEntity1 = new ScheduleEntity
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            FixedTask = fixedTask1,
            CreatedTimestamp = DateTime.Now
        };
        var scheduleEntity2 = new ScheduleEntity
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            FixedTask = fixedTask2,
            CreatedTimestamp = DateTime.Now
        };
        _scheduleEntities.Add(scheduleEntity1);
        _scheduleEntities.Add(scheduleEntity2);

        await _fixedTaskAppService.DeleteAsync(fixedTask1Id, TestContext.Current.CancellationToken);

        _scheduleEntities.Should().NotContain(x => x.FixedTask != null && x.FixedTask.Id == fixedTask1Id);
        _scheduleEntities.Should().Contain(x => x.FixedTask != null && x.FixedTask.Id == fixedTask2Id);
    }

    [Fact]
    [Trait("DeleteAsync", "Should throw NotFoundException when deleting non-existent task")]
    public async Task DeleteAsync_WithNonExistentId_ShouldThrowNotFound()
    {
        var nonExistentId = Guid.NewGuid();

        var act = async () => await _fixedTaskAppService.DeleteAsync(nonExistentId, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    [Trait("GetAll", "Should return correct data")]
    public async Task GetAll_ShouldReturnCorrectData()
    {
        var result = await _fixedTaskAppService.GetAll(TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        result.Count.Should().Be(_fixedTasks.Count);
        result.Should().BeEquivalentTo(_fixedTasks.Select(FixedTaskDto.Create).ToList());
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return correct data")]
    public async Task GetByIdAsync_ShouldUpdateEntry()
    {
        var id = _fixedTasks.First(x => x.UserId == _userId).Id;
        var result = await _fixedTaskAppService.GetByIdAsync(id, TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    [Trait("AddAsync", "Should link the given categories")]
    public async Task AddAsync_ShouldLinkCategories()
    {
        var categoryId = OwnCategoryId;

        await _fixedTaskAppService.AddAsync(new FixedTaskDto
        {
            Name = "Categorised",
            Categories = [LinkCategoryDto.EmptyLink(categoryId)]
        }, TestContext.Current.CancellationToken);

        _fixedTasks.Single(x => x.Name == "Categorised")
            .CategoryFixedTasks.Should().ContainSingle(link => link.CategoryId == categoryId);
    }

    [Fact]
    [Trait("AddAsync", "Should collapse a repeated category id")]
    public async Task AddAsync_ShouldNotLinkTheSameCategoryTwice()
    {
        var categoryId = OwnCategoryId;

        await _fixedTaskAppService.AddAsync(new FixedTaskDto
        {
            Name = "Categorised",
            Categories = [LinkCategoryDto.EmptyLink(categoryId), LinkCategoryDto.EmptyLink(categoryId)]
        }, TestContext.Current.CancellationToken);

        _fixedTasks.Single(x => x.Name == "Categorised").CategoryFixedTasks.Should().HaveCount(1);
    }

    [Fact]
    [Trait("UpdateAsync", "Should replace the linked categories")]
    public async Task UpdateAsync_ShouldReplaceCategories()
    {
        var task = _fixedTasks.First(x => x.UserId == _userId);
        var categoryId = OwnCategoryId;
        task.CategoryFixedTasks.Add(new CategoryFixedTask { CategoryId = Guid.NewGuid(), FixedTaskId = task.Id });

        await _fixedTaskAppService.UpdateAsync(new FixedTaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Categories = [LinkCategoryDto.EmptyLink(categoryId)]
        }, TestContext.Current.CancellationToken);

        task.CategoryFixedTasks.Should().ContainSingle(link => link.CategoryId == categoryId);
    }

    [Fact]
    [Trait("UpdateAsync", "Should unlink every category when none is given")]
    public async Task UpdateAsync_ShouldClearCategories_WhenNoneGiven()
    {
        var task = _fixedTasks.First(x => x.UserId == _userId);
        task.CategoryFixedTasks.Add(new CategoryFixedTask { CategoryId = OwnCategoryId, FixedTaskId = task.Id });

        await _fixedTaskAppService.UpdateAsync(new FixedTaskDto
        {
            Id = task.Id,
            Name = task.Name
        }, TestContext.Current.CancellationToken);

        task.CategoryFixedTasks.Should().BeEmpty();
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

        var add = async () => await _fixedTaskAppService.AddAsync(
            new FixedTaskDto { Name = "Sneaky", Categories = categories }, TestContext.Current.CancellationToken);
        (await add.Should().ThrowAsync<NotFoundException>()).Which.ResourceName.Should().Be("Category");

        var update = async () => await _fixedTaskAppService.UpdateAsync(
            new FixedTaskDto { Id = _fixedTasks.First(x => x.UserId == _userId).Id, Name = "Sneaky", Categories = categories },
            TestContext.Current.CancellationToken);
        (await update.Should().ThrowAsync<NotFoundException>()).Which.ResourceName.Should().Be("Category");

        _fixedTasks.Should().NotContain(x => x.Name == "Sneaky");
    }

    // Validation Tests
    [Fact]
    [Trait("AddAsync", "Should throw on null input")]
    public async Task AddAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _fixedTaskAppService.AddAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("UpdateAsync", "Should throw on null input")]
    public async Task UpdateAsync_ShouldThrowNotProvidedException_WhenNullInput()
    {
        await Assert.ThrowsAsync<NotProvidedException>(() =>
            _fixedTaskAppService.UpdateAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("GetByIdAsync", "Should return null for non-existent ID")]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNonExistentId()
    {
        var result = await _fixedTaskAppService.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);
        result.Should().BeNull();
    }

    #region Mock helpers

    private void SetupMocks(Guid userId)
    {
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

        _fixedTasksRepository.As<IUserScopedRepositoryBase<FixedTask, Guid>>().SetupRepositoryMock(_fixedTasks);

        // Setup ScheduleEntity repository for cascade delete testing
        _scheduleEntities = [];

        _scheduleEntityRepository.As<IUserScopedRepositoryBase<ScheduleEntity, Guid>>().SetupRepositoryMock(_scheduleEntities);

        _categories =
        [
            new() { UserId = userId, Name = "Own category" },
            new() { UserId = Guid.NewGuid(), Name = "Another user's category" }
        ];
        _categoryRepository.As<IUserScopedRepositoryBase<Category, Guid>>().SetupRepositoryMock(_categories, userId);

        // Setup DeleteBy method for code-side cascade delete
        _scheduleEntityRepository.Setup(x => x.DeleteBy(It.IsAny<Expression<Func<ScheduleEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .Returns<Expression<Func<ScheduleEntity, bool>>, CancellationToken>((predicate, _) =>
            {
                var compiled = predicate.Compile();
                var deletedCount = _scheduleEntities.RemoveAll(e => compiled(e));
                return Task.FromResult(deletedCount);
            });
    }

    #endregion
}
