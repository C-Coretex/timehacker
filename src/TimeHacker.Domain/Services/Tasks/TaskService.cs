using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.ReturnModels;
using TimeHacker.Domain.Processors;

namespace TimeHacker.Domain.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly IFixedTaskService _fixedTaskService;
        private readonly IDynamicTaskService _dynamicTaskService;
        private readonly IScheduleSnapshotService _scheduleSnapshotService;

        private readonly TaskTimelineProcessor _taskTimelineProcessor;

        public TaskService(IFixedTaskService fixedTaskService, IDynamicTaskService dynamicTaskService, IScheduleSnapshotService scheduleSnapshotService)
        {
            _fixedTaskService = fixedTaskService;
            _dynamicTaskService = dynamicTaskService;
            _scheduleSnapshotService = scheduleSnapshotService;

            _taskTimelineProcessor = new TaskTimelineProcessor();
        }

        public async Task<TasksForDayReturn> GetTasksForDay(DateTime date)
        {
            var snapshot = await _scheduleSnapshotService.GetBy(DateOnly.FromDateTime(date));
            if(snapshot?.ScheduleData != null)
                return snapshot.ScheduleData;

            snapshot = new ScheduleSnapshot()
            {
                Date = DateOnly.FromDateTime(date),
            };

            var fixedTasks = _fixedTaskService.GetAll()
                                              .Where(ft => ft.StartTimestamp.Date == date.Date)
                                              .OrderBy(ft => ft.StartTimestamp)
                                              .AsEnumerable();

            var dynamicTasks = _dynamicTaskService.GetAll().AsEnumerable();

            snapshot.ScheduleData = _taskTimelineProcessor.GetTasksForDay(fixedTasks, dynamicTasks, date);

            await _scheduleSnapshotService.Add(snapshot);
            return snapshot.ScheduleData;
        }

        public async IAsyncEnumerable<TasksForDayReturn> GetTasksForDays(IEnumerable<DateTime> dates)
        {
            var fixedTasks = await _fixedTaskService.GetAll()
                                                    .Where(ft => dates.Any(d => d.Date == ft.StartTimestamp.Date))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await _dynamicTaskService.GetAll().ToListAsync();

            foreach (var date in dates)
            {
                var snapshot = await _scheduleSnapshotService.GetBy(DateOnly.FromDateTime(date));
                if (snapshot?.ScheduleData != null)
                    yield return snapshot.ScheduleData;
                else
                {
                    snapshot = new ScheduleSnapshot()
                    {
                        Date = DateOnly.FromDateTime(date),
                    };

                    var fixedTasksForDay = fixedTasks.Where(ft => ft.StartTimestamp.Date == date.Date);
                    snapshot.ScheduleData = _taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, dynamicTasks, date);

                    await _scheduleSnapshotService.Add(snapshot);
                    yield return snapshot.ScheduleData;
                }
            }
        }

        public async IAsyncEnumerable<TasksForDayReturn> RefreshTasksForDays(IEnumerable<DateTime> dates)
        {
            var fixedTasks = await _fixedTaskService.GetAll()
                                                    .Where(ft => dates.Any(d => d.Date == ft.StartTimestamp.Date))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await _dynamicTaskService.GetAll().ToListAsync();

            foreach (var date in dates)
            {
                var snapshot = await _scheduleSnapshotService.GetBy(DateOnly.FromDateTime(date));
                snapshot ??= new ScheduleSnapshot()
                {
                    Date = DateOnly.FromDateTime(date),
                };

                var fixedTasksForDay = fixedTasks.Where(ft => ft.StartTimestamp.Date == date.Date);
                snapshot.ScheduleData = _taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, dynamicTasks, date);

                if(snapshot.UserId == null)
                    await _scheduleSnapshotService.Add(snapshot);
                else
                    await _scheduleSnapshotService.Update(snapshot);

                yield return snapshot.ScheduleData;
            }
        }
    }
}
