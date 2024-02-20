using Helpers.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Security.Claims;
using TimeHacker.Application.Models.PageModels;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Persistence.Tasks;
using static TimeHacker.Application.Pages.IndexModel;
using TimeHacker.Persistence.Services.Tasks;

namespace TimeHacker.Application.Pages
{
    public class TasksModel : PageModel
    {
        private readonly ILogger<TasksModel> _logger;
        private readonly DynamicTasksService _dynamicTasksService;
        private readonly FixedTasksService _fixedTasksService;
        private readonly string _userId;

        public InputDynamicTaskModel InputDynamicTaskModel { get; set; }
        public InputFixedTaskModel InputFixedTaskModel { get; set; }
        public IEnumerable<DynamicTask> DynamicTasks { get; set; } = Enumerable.Empty<DynamicTask>();
        public IEnumerable<FixedTask> FixedTasks { get; set; } = Enumerable.Empty<FixedTask>();

        public TasksModel(ILogger<TasksModel> logger, DynamicTasksService dynamicTasksService, FixedTasksService fixedTasksService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dynamicTasksService = dynamicTasksService;
            _fixedTasksService = fixedTasksService;
            var user = httpContextAccessor.HttpContext?.User;
            _userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not found");
        }

        public void OnGet()
        {
            DynamicTasks = _dynamicTasksService.Queries.GetAllByUserId(_userId).AsNoTracking();
            FixedTasks = _fixedTasksService.Queries.GetAllByUserId(_userId).AsNoTracking();
        }

        public async Task<IActionResult> OnPostDynamicTaskFormHandler(int id, InputDynamicTaskModel inputDynamicTaskModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var dynamicTask = new DynamicTask
                {
                    Id = id,
                    UserId = _userId,
                    Name = inputDynamicTaskModel.Name,
                    Description = inputDynamicTaskModel.Description,
                    Category = inputDynamicTaskModel.Category,
                    Priority = inputDynamicTaskModel.Priority,
                    MinTimeToFinish = inputDynamicTaskModel.MinTimeToFinish,
                    MaxTimeToFinish = inputDynamicTaskModel.MaxTimeToFinish,
                    OptimalTimeToFinish = inputDynamicTaskModel.OptimalTimeToFinish
                };

                if (!dynamicTask.IsObjectValid(out var validationResults))
                {
                    foreach (var validationResult in validationResults)
                    {
                        ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage ?? "");
                    }

                    return Page();
                }

                await _dynamicTasksService.Commands.UpdateAsync(dynamicTask);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding dynamic task");
                throw;
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostFixedTaskFormHandler(int id, InputFixedTaskModel inputFixedTaskModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var fixedTask = new FixedTask
                {
                    Id = id,
                    UserId = _userId,
                    Name = inputFixedTaskModel.Name,
                    Description = inputFixedTaskModel.Description,
                    Category = inputFixedTaskModel.Category,
                    Priority = inputFixedTaskModel.Priority,
                    StartTimestamp = DateTime.ParseExact(inputFixedTaskModel.StartTimestamp, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                    EndTimestamp = DateTime.ParseExact(inputFixedTaskModel.EndTimestamp, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)
                };

                if (!fixedTask.IsObjectValid(out var validationResults))
                {
                    foreach (var validationResult in validationResults)
                    {
                        ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage ?? "");
                    }

                    return Page();
                }

                await _fixedTasksService.Commands.UpdateAsync(fixedTask);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding fixed task");
                throw;
            }

            return RedirectToPage();
        }
    }
}
