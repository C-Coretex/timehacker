using Microsoft.EntityFrameworkCore;
using Timehacker.Integration.Db.Tests;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Integration.Db.Tests.Fixtures;

namespace TimeHacker.Integration.Db.Tests.ServiceFlowTests;

// FixedTaskAppService.DeleteAsync removes the related ScheduleEntity first, then the task. Combined with
// the DB cascades this must clear the task, its schedule, and any generated scheduled instances, while
// leaving unrelated data intact.
public class FixedTaskCascadeDeleteTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("DeleteAsync", "CascadesScheduleAndInstances")]
    public async Task DeleteAsync_Should_RemoveTaskScheduleAndScheduledInstances()
    {
        var graphSeeder = Resolve<GraphSeeder>();
        var task = await graphSeeder.SeedFixedTaskWithSchedule(TestContext.Current.CancellationToken);
        var scheduleEntityId = task.ScheduleEntityId!.Value;

        // A generated scheduled instance pointing at the schedule entity.
        var snapshot = await graphSeeder.SeedSnapshotWithScheduledInstanceFor(scheduleEntityId, new DateOnly(2026, 7, 1), TestContext.Current.CancellationToken);

        // An unrelated task that must survive.
        var unrelatedTask = await graphSeeder.SeedUnrelatedFixedTask(TestContext.Current.CancellationToken);

        await Resolve<IFixedTaskAppService>().DeleteAsync(task.Id, TestContext.Current.CancellationToken);

        Db.ChangeTracker.Clear();
        (await Db.Set<FixedTask>().AnyAsync(x => x.Id == task.Id, TestContext.Current.CancellationToken)).Should().BeFalse();
        (await Db.Set<ScheduleEntity>().AnyAsync(x => x.Id == scheduleEntityId, TestContext.Current.CancellationToken)).Should().BeFalse();
        (await Db.Set<ScheduledTask>().AnyAsync(t => t.ParentScheduleEntityId == scheduleEntityId, TestContext.Current.CancellationToken)).Should().BeFalse();

        // Unrelated task and the snapshot itself are untouched.
        (await Db.Set<FixedTask>().AnyAsync(x => x.Id == unrelatedTask.Id, TestContext.Current.CancellationToken)).Should().BeTrue();
        (await Db.Set<ScheduleSnapshot>().AnyAsync(s => s.Id == snapshot.Id, TestContext.Current.CancellationToken)).Should().BeTrue();
    }
}
