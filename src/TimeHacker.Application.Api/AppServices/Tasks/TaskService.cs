using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Application.Api.QueryPipelineSteps;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Application.Api.AppServices.Tasks
{
    public class TaskService(
        IFixedTaskRepository fixedTaskRepository,
        IDynamicTaskRepository dynamicTaskRepository,
        IScheduleSnapshotRepository scheduleSnapshotRepository,
        IScheduleEntityService scheduleEntityService,
        ITaskTimelineProcessor taskTimelineProcessor) : ITaskAppService
    {
        public async Task<TasksForDayDto> GetTasksForDay(DateOnly date)
        {
            var snapshot = await GetSnapshotForDate(date);
            if (snapshot != null)
                return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));

            var fixedTasks = await fixedTaskRepository.GetAll()
                                              .Where(ft => DateOnly.FromDateTime(ft.StartTimestamp) == date)
                                              .OrderBy(ft => ft.StartTimestamp)
                                              .ToListAsync();

            var dynamicTasks = await dynamicTaskRepository.GetAll().ToListAsync();

            var scheduledFixedTasks = await GetFixedTasksForScheduledTasks(date).ToListAsync();

            var tasksForDay = taskTimelineProcessor.GetTasksForDay(fixedTasks, scheduledFixedTasks, dynamicTasks, date);

            snapshot = tasksForDay.CreateScheduleSnapshot();
            snapshot = await scheduleSnapshotRepository.AddAndSaveAsync(snapshot);

            return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
        }

        public async IAsyncEnumerable<TasksForDayDto> GetTasksForDays(ICollection<DateOnly> dates)
        {
            var fixedTasks = await fixedTaskRepository.GetAll()
                                                    .Where(ft => dates.Any(d => d == DateOnly.FromDateTime(ft.StartTimestamp)))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await dynamicTaskRepository.GetAll().ToListAsync();

            var scheduledFixedTasks = await GetFixedTasksForScheduledTasks(dates.Min(), dates.Max()).ToListAsync();

            foreach (var date in dates)
            {
                var snapshot = await GetSnapshotForDate(date);
                if (snapshot == null)
                {
                    var fixedTasksForDay = fixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                    var scheduledFixedTasksForDay = scheduledFixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                    var tasksForDay = taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, scheduledFixedTasksForDay, dynamicTasks, date);

                    snapshot = tasksForDay.CreateScheduleSnapshot();
                    snapshot = scheduleSnapshotRepository.Add(snapshot);
                }
                yield return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
            }

            await scheduleSnapshotRepository.SaveChangesAsync();
        }

        public async IAsyncEnumerable<TasksForDayDto> RefreshTasksForDays(ICollection<DateOnly> dates)
        {
            var fixedTasks = await fixedTaskRepository.GetAll()
                                                    .Where(ft => dates.Any(d => d == DateOnly.FromDateTime(ft.StartTimestamp)))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await dynamicTaskRepository.GetAll().ToListAsync();

            var scheduledFixedTasks = await GetFixedTasksForScheduledTasks(dates.Min(), dates.Max()).ToListAsync();

            foreach (var date in dates)
            {
                var snapshot = await GetSnapshotForDate(date);
                if (snapshot != null)
                    await scheduleSnapshotRepository.Update(snapshot);
                else
                {
                    snapshot = new ScheduleSnapshot();
                    scheduleSnapshotRepository.Add(snapshot);
                }

                var fixedTasksForDay = fixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                var scheduledFixedTasksForDay = scheduledFixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date).ToList();
                var tasksForDay = taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, scheduledFixedTasksForDay, dynamicTasks, date);

                snapshot = tasksForDay.CreateScheduleSnapshot(snapshot);


                foreach(var scheduledFixedTasksForDayEntry in scheduledFixedTasksForDay)
                    await scheduleEntityService.UpdateLastEntityCreated(scheduledFixedTasksForDayEntry.ScheduleEntityId!.Value, date);

                yield return TasksForDayDto.Create(TasksForDayReturn.Create(snapshot));
            }

            await scheduleSnapshotRepository.SaveChangesAsync();
        }

        private Task<ScheduleSnapshot?> GetSnapshotForDate(DateOnly date)
        {
            return scheduleSnapshotRepository.GetAll(QueryPipelineScheduleSnapshots.IncludeScheduledData)
                .FirstOrDefaultAsync(x => x.Date == date);
        }

        private async IAsyncEnumerable<FixedTask> GetFixedTasksForScheduledTasks(DateOnly from, DateOnly? to = null)
        {
            var scheduleEntities = scheduleEntityService.GetAllFrom(from).AsAsyncEnumerable();

            await foreach (var scheduleEntity in scheduleEntities)
            {
                var taskDates = scheduleEntity.GetNextEntityDatesIn(from, to ?? from);

                foreach (var taskDate in taskDates)
                {
                    var task = scheduleEntity.FixedTask!.ShallowCopy();
                    var timeDifference = task.EndTimestamp.Date - task.StartTimestamp.Date;

                    task.StartTimestamp = taskDate.ToDateTime(TimeOnly.FromDateTime(task.StartTimestamp));
                    task.EndTimestamp = taskDate.AddDays(timeDifference.Days).ToDateTime(TimeOnly.FromDateTime(task.StartTimestamp));

                    yield return task;
                }
            }
        }
    }
}
