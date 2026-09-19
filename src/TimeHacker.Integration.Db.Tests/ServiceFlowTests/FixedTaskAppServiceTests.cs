using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.BusinessLogicExceptions;

namespace TimeHacker.Integration.Db.Tests.ServiceFlowTests;

public class FixedTaskAppServiceTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;
    private IFixedTaskAppService Service => Resolve<IFixedTaskAppService>();

    private Task<List<Guid>> LinkedCategoryIdsOf(Guid taskId)
        => Db.Set<CategoryFixedTask>()
            .Where(x => x.FixedTaskId == taskId)
            .Select(x => x.CategoryId)
            .ToListAsync(Ct);

    [Fact]
    [Trait("GetAll", "ReturnsCorrectScheduleEntries")]
    public async Task GetAll_Should_MapScheduledEntriesCorrectly()
    {
        var task = await Seeder.SeedFixedTaskWithSchedule(Ct);
        var scheduleEntityId = task.ScheduleEntityId!.Value;

        // An unrelated task that must survive.
        var unrelatedTask = await Seeder.SeedUnrelatedFixedTask(Ct);

        var tasks = await Service.GetAll(Ct).ToListAsync(Ct);

        tasks.Select(x => x.Id).Should().BeEquivalentTo([task.Id, unrelatedTask.Id]);
        tasks.Single(x => x.Id == task.Id).ScheduleEntity!.Id.Should().Be(scheduleEntityId);
        tasks.Single(x => x.Id == unrelatedTask.Id).ScheduleEntity.Should().BeNull();
    }

    [Fact]
    [Trait("GetAll", "ReturnsCorrectCategoryEntries")]
    public async Task GetAll_Should_MapCategoryEntriesCorrectly()
    {
        var (category, _, task) = await Seeder.SeedFixedTaskWithCategoryAndTagJunctions(Ct);

        var tasks = await Service.GetAll(Ct).ToListAsync(Ct);

        var linked = tasks.Should().ContainSingle(t => t.Id == task.Id).Subject.RichCategories.Should().ContainSingle().Subject;
        linked.Id.Should().Be(category.Id);
        linked.Name.Should().Be(category.Name);
        // System.Drawing.Color equality is name-sensitive, and the round trip through the DB is a bare ARGB int.
        linked.Color.ToArgb().Should().Be(category.Color.ToArgb());
    }

    [Fact]
    [Trait("GetAll", "ReturnsCategorySchedules")]
    public async Task GetAll_Should_PopulateCategorySchedules()
    {
        var (window, task) = await Seeder.SeedFixedTaskLinkedToScheduledCategory(Ct);

        var tasks = await Service.GetAll(Ct).ToListAsync(Ct);

        var category = tasks.Single(t => t.Id == task.Id).RichCategories.Should().ContainSingle().Subject;
        category.Id.Should().Be(window.CategoryId);
        category.Schedules.Should().ContainSingle(s => s.Id == window.Id);
    }

    [Fact]
    [Trait("GetByIdAsync", "ReturnsCorrectCategoryEntries")]
    public async Task GetByIdAsync_Should_MapCategoryEntriesCorrectly()
    {
        var (category, _, task) = await Seeder.SeedFixedTaskWithCategoryAndTagJunctions(Ct);

        var found = await Service.GetByIdAsync(task.Id, Ct);

        found!.RichCategories.Should().ContainSingle(c => c.Id == category.Id && c.Name == category.Name);
    }

    [Fact]
    [Trait("GetByIdAsync", "ReturnsCategorySchedules")]
    public async Task GetByIdAsync_Should_PopulateCategorySchedules()
    {
        var (window, task) = await Seeder.SeedFixedTaskLinkedToScheduledCategory(Ct);

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

        var id = await Service.AddAsync(new FixedTaskDto
        {
            Name = "Test Task",
            Categories = [LinkCategoryDto.EmptyLink(first.Id), LinkCategoryDto.EmptyLink(second.Id)]
        }, Ct);

        (await LinkedCategoryIdsOf(id)).Should().BeEquivalentTo([first.Id, second.Id]);
    }

    [Fact]
    [Trait("AddAsync", "AddsNoCategoriesWhenNoneGiven")]
    public async Task AddAsync_Should_LinkNothing_WhenNoCategoriesGiven()
    {
        var id = await Service.AddAsync(new FixedTaskDto { Name = "Uncategorised" }, Ct);

        (await LinkedCategoryIdsOf(id)).Should().BeEmpty();
    }

    [Fact]
    [Trait("UpdateAsync", "UpdatesCategoriesCorrectly")]
    public async Task UpdateAsync_Should_AddAndRemoveCategories()
    {
        var (category, _, task) = await Seeder.SeedFixedTaskWithCategoryAndTagJunctions(Ct);
        var second = await Seeder.SeedCategoryForCurrentUser("Second", Ct);

        var dto = FixedTaskDto.Create(task)! with
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
    [Trait("UpdateAsync", "ClearsCategoriesWhenNoneGiven")]
    public async Task UpdateAsync_Should_ClearCategories_WhenNoneGiven()
    {
        var (_, _, task) = await Seeder.SeedFixedTaskWithCategoryAndTagJunctions(Ct);

        var dto = FixedTaskDto.Create(task)! with { Categories = [] };
        await Service.UpdateAsync(dto, Ct);

        (await LinkedCategoryIdsOf(task.Id)).Should().BeEmpty();
    }

    [Fact]
    [Trait("UpdateAsync", "LeavesTheCategoryItself")]
    public async Task UpdateAsync_Should_UnlinkWithoutDeletingTheCategory()
    {
        var (category, _, task) = await Seeder.SeedFixedTaskWithCategoryAndTagJunctions(Ct);

        var dto = FixedTaskDto.Create(task)! with { Categories = [] };
        await Service.UpdateAsync(dto, Ct);

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

        var act = async () => await Service.AddAsync(new FixedTaskDto { Name = "Sneaky", Categories = categories }, Ct);

        (await act.Should().ThrowAsync<NotFoundException>()).Which.ResourceName.Should().Be("Category");
        (await Db.Set<CategoryFixedTask>().AnyAsync(x => x.CategoryId == foreign.Id, Ct)).Should().BeFalse();
    }
}
