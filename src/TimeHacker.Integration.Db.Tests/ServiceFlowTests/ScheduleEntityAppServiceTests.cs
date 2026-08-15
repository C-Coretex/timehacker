using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Integration.Db.Tests.ServiceFlowTests;

// ScheduleEntityAppService.Save creates the ScheduleEntity and points the parent's FK at it via a
// targeted UpdateProperty.
public class ScheduleEntityAppServiceTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("Save", "FixedTaskParent")]
    public async Task Save_Should_CreateScheduleAndLinkFixedTask()
    {
        var task = new FixedTask
        {
            Name = "Task",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)
        };
        await Resolve<IFixedTaskRepository>().AddAndSaveAsync(task, TestContext.Current.CancellationToken);

        var dto = new ScheduleEntityCreateDto(ScheduleEntityParentType.FixedTask, task.Id, GraphSeeder.DailyRepeat());
        var result = await Resolve<IScheduleEntityAppService>().Save(dto, TestContext.Current.CancellationToken);

        result.Id.Should().NotBeNull();
        Db.ChangeTracker.Clear();
        var reloaded = await Db.Set<FixedTask>().FirstAsync(x => x.Id == task.Id, TestContext.Current.CancellationToken);
        reloaded.ScheduleEntityId.Should().Be(result.Id);
    }

    [Fact]
    [Trait("Save", "CategoryParent")]
    public async Task Save_Should_CreateScheduleAndLinkCategory()
    {
        var category = new Category { Name = "Cat", Color = Color.SeaGreen };
        await Resolve<ICategoryRepository>().AddAndSaveAsync(category, TestContext.Current.CancellationToken);

        var dto = new ScheduleEntityCreateDto(ScheduleEntityParentType.Category, category.Id, GraphSeeder.DailyRepeat());
        var result = await Resolve<IScheduleEntityAppService>().Save(dto, TestContext.Current.CancellationToken);

        result.Id.Should().NotBeNull();
        Db.ChangeTracker.Clear();
        var reloaded = await Db.Set<Category>().FirstAsync(x => x.Id == category.Id, TestContext.Current.CancellationToken);
        reloaded.ScheduleEntityId.Should().Be(result.Id);
    }

    [Fact]
    [Trait("Save", "MissingParentThrows")]
    public async Task Save_WithUnknownParent_Should_ThrowNotFound()
    {
        var dto = new ScheduleEntityCreateDto(ScheduleEntityParentType.FixedTask, Guid.NewGuid(), GraphSeeder.DailyRepeat());

        var act = async () => await Resolve<IScheduleEntityAppService>().Save(dto, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
