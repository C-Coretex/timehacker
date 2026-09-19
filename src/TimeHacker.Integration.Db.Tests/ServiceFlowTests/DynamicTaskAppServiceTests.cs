using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.BusinessLogicExceptions;

namespace TimeHacker.Integration.Db.Tests.ServiceFlowTests;

public class DynamicTaskAppServiceTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;
    private IDynamicTaskAppService Service => Resolve<IDynamicTaskAppService>();

    private static DynamicTaskDto NewTask(string name, params Guid[] categoryIds) => new()
    {
        Name = name,
        MinTimeToFinish = TimeSpan.FromMinutes(30),
        MaxTimeToFinish = TimeSpan.FromMinutes(60),
        Categories = [.. categoryIds.Select(LinkCategoryDto.EmptyLink)]
    };

    private Task<List<Guid>> LinkedCategoryIdsOf(Guid taskId)
        => Db.Set<CategoryDynamicTask>()
            .Where(x => x.DynamicTaskId == taskId)
            .Select(x => x.CategoryId)
            .ToListAsync(Ct);

    [Fact]
    [Trait("GetAll", "ReturnsCorrectCategoryEntries")]
    public async Task GetAll_Should_MapCategoryEntriesCorrectly()
    {
        var (category, _, task) = await Seeder.SeedDynamicTaskWithCategoryAndTagJunctions(Ct);

        var tasks = await Service.GetAll(Ct).ToListAsync(Ct);

        var linked = tasks.Should().ContainSingle(t => t.Id == task.Id).Subject.RichCategories.Should().ContainSingle().Subject;
        linked.Id.Should().Be(category.Id);
        linked.Name.Should().Be(category.Name);
        // System.Drawing.Color equality is name-sensitive, and the round trip through the DB is a bare ARGB int.
        linked.Color.ToArgb().Should().Be(category.Color.ToArgb());
    }

    [Fact]
    [Trait("GetByIdAsync", "ReturnsCorrectCategoryEntries")]
    public async Task GetByIdAsync_Should_MapCategoryEntriesCorrectly()
    {
        var (category, _, task) = await Seeder.SeedDynamicTaskWithCategoryAndTagJunctions(Ct);

        var found = await Service.GetByIdAsync(task.Id, Ct);

        found!.RichCategories.Should().ContainSingle(c => c.Id == category.Id && c.Name == category.Name);
    }

    [Fact]
    [Trait("GetAll", "ReturnsCategorySchedules")]
    public async Task GetAll_Should_PopulateCategorySchedules()
    {
        var (window, task) = await Seeder.SeedDynamicTaskLinkedToScheduledCategory(Ct);

        var tasks = await Service.GetAll(Ct).ToListAsync(Ct);

        var category = tasks.Single(t => t.Id == task.Id).RichCategories.Should().ContainSingle().Subject;
        category.Id.Should().Be(window.CategoryId);
        category.Schedules.Should().ContainSingle(s => s.Id == window.Id);
    }

    [Fact]
    [Trait("GetByIdAsync", "ReturnsCategorySchedules")]
    public async Task GetByIdAsync_Should_PopulateCategorySchedules()
    {
        var (window, task) = await Seeder.SeedDynamicTaskLinkedToScheduledCategory(Ct);

        var found = await Service.GetByIdAsync(task.Id, Ct);

        var category = found!.RichCategories.Should().ContainSingle().Subject;
        category.Id.Should().Be(window.CategoryId);
        category.Schedules.Should().ContainSingle(s => s.Id == window.Id);
    }

    [Fact]
    [Trait("AddAsync", "AddsCategoriesCorrectly")]
    public async Task AddAsync_Should_AddCategoriesCorrectly()
    {
        var first = await Seeder.SeedCategoryForCurrentUser("First", Ct);
        var second = await Seeder.SeedCategoryForCurrentUser("Second", Ct);

        var id = await Service.AddAsync(NewTask("Test Task", first.Id, second.Id), Ct);

        (await LinkedCategoryIdsOf(id)).Should().BeEquivalentTo([first.Id, second.Id]);
    }

    [Fact]
    [Trait("AddAsync", "RejectsATaskWithNoCategory")]
    public async Task AddAsync_Should_Throw_WhenNoCategoriesGiven()
    {
        // Unlike a fixed task, a dynamic one must always carry at least one category.
        var act = async () => await Service.AddAsync(NewTask("Uncategorised"), Ct);

        await act.Should().ThrowAsync<DataIsNotCorrectException>();
        (await Db.Set<DynamicTask>().AnyAsync(t => t.Name == "Uncategorised", Ct)).Should().BeFalse();
    }

    [Fact]
    [Trait("UpdateAsync", "UpdatesCategoriesCorrectly")]
    public async Task UpdateAsync_Should_AddAndRemoveCategories()
    {
        var (category, _, task) = await Seeder.SeedDynamicTaskWithCategoryAndTagJunctions(Ct);
        var second = await Seeder.SeedCategoryForCurrentUser("Second", Ct);

        var dto = DynamicTaskDto.Create(task)! with
        {
            Categories = [LinkCategoryDto.EmptyLink(category.Id), LinkCategoryDto.EmptyLink(second.Id)]
        };
        await Service.UpdateAsync(dto, Ct);

        (await LinkedCategoryIdsOf(task.Id)).Should().BeEquivalentTo([category.Id, second.Id]);

        // Dropping the original and adding a third: the update replaces the set rather than adding to it.
        var third = await Seeder.SeedCategoryForCurrentUser("Third", Ct);
        dto = dto with { Categories = [LinkCategoryDto.EmptyLink(second.Id), LinkCategoryDto.EmptyLink(third.Id)] };
        await Service.UpdateAsync(dto, Ct);

        (await LinkedCategoryIdsOf(task.Id)).Should().BeEquivalentTo([second.Id, third.Id]);
    }

    [Fact]
    [Trait("UpdateAsync", "RejectsClearingEveryCategory")]
    public async Task UpdateAsync_Should_Throw_WhenNoCategoriesGiven()
    {
        var (category, _, task) = await Seeder.SeedDynamicTaskWithCategoryAndTagJunctions(Ct);

        var act = async () => await Service.UpdateAsync(DynamicTaskDto.Create(task)! with { Categories = [] }, Ct);

        await act.Should().ThrowAsync<DataIsNotCorrectException>();
        // The rejected update left the existing link in place.
        (await LinkedCategoryIdsOf(task.Id)).Should().BeEquivalentTo([category.Id]);
    }

    [Fact]
    [Trait("UpdateAsync", "LeavesTheCategoryItself")]
    public async Task UpdateAsync_Should_UnlinkWithoutDeletingTheCategory()
    {
        var (category, _, task) = await Seeder.SeedDynamicTaskWithCategoryAndTagJunctions(Ct);
        var replacement = await Seeder.SeedCategoryForCurrentUser("Replacement", Ct);

        // Swapped rather than cleared: a dynamic task can never be left with none.
        var dto = DynamicTaskDto.Create(task)! with { Categories = [LinkCategoryDto.EmptyLink(replacement.Id)] };
        await Service.UpdateAsync(dto, Ct);

        (await LinkedCategoryIdsOf(task.Id)).Should().BeEquivalentTo([replacement.Id]);
        (await Db.Set<Category>().AnyAsync(c => c.Id == category.Id, Ct)).Should().BeTrue();
    }

    [Fact]
    [Trait("AddAsync", "RejectsACategoryTheUserCannotSee")]
    public async Task AddAsync_Should_Throw_ForAnotherUsersCategory()
    {
        // PostgreSQL runs foreign-key checks as the table owner, so RLS does not stop the junction insert
        // on its own: the guard in the app service is what keeps the link out.
        var foreign = await OtherUsers.First().Resolve<ICategoryRepository>()
            .AddAndSaveAsync(new Category { Name = "Foreign", Color = Color.Red }, Ct);
        var categories = new[] { LinkCategoryDto.EmptyLink(foreign.Id) };

        var act = async () => await Service.AddAsync(NewTask("Sneaky") with { Categories = categories }, Ct);

        (await act.Should().ThrowAsync<NotFoundException>()).Which.ResourceName.Should().Be("Category");
        (await Db.Set<CategoryDynamicTask>().AnyAsync(x => x.CategoryId == foreign.Id, Ct)).Should().BeFalse();
    }
}
