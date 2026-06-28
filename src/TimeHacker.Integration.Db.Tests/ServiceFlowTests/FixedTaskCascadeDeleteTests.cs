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
        var userId = CurrentUser.UserId;
        var task = await Resolve<GraphSeeder>().SeedFixedTaskWithSchedule(TestContext.Current.CancellationToken);
        var scheduleEntityId = task.ScheduleEntityId!.Value;

        // A generated scheduled instance pointing at the schedule entity.
        var date = new DateOnly(2026, 7, 1);
        var snapshot = new ScheduleSnapshot
        {
            UserId = userId,
            Date = date,
            ScheduledTasks =
            {
                new ScheduledTask { UserId = userId, Date = date, Name = "instance", IsFixed = true, ParentScheduleEntityId = scheduleEntityId }
            }
        };
        Db.Add(snapshot);

        // An unrelated task that must survive.
        var unrelatedTask = new FixedTask
        {
            UserId = userId,
            Name = "Unrelated",
            Priority = 1,
            StartTimestamp = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
            EndTimestamp = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)
        };
        Db.Add(unrelatedTask);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);

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
