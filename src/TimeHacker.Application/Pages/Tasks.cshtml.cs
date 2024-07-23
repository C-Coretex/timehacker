using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Security.Claims;
using TimeHacker.Application.Models.Input.Tasks;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Helpers.Domain.Extensions;

namespace TimeHacker.Application.Pages
{
    [Authorize]
    public class TasksModel : PageModel
    {
        private readonly ILogger<TasksModel> _logger;
        private readonly IDynamicTaskService _dynamicTasksService;
        private readonly IFixedTaskService _fixedTasksService;
        private readonly string _userId;
        private readonly bool _isSignedIn;

        public InputDynamicTaskModel InputDynamicTaskModel { get; set; }
        public InputFixedTaskModel InputFixedTaskModel { get; set; }
        public IEnumerable<DynamicTask> DynamicTasks { get; set; } = [];
        public IEnumerable<FixedTask> FixedTasks { get; set; } = [];

        public TasksModel(ILogger<TasksModel> logger, IDynamicTaskService dynamicTasksService, IFixedTaskService fixedTasksService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dynamicTasksService = dynamicTasksService;
            _fixedTasksService = fixedTasksService;
            var user = httpContextAccessor.HttpContext?.User;
            _userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not found");
        }

        public void OnGet()
        {
            DynamicTasks = _dynamicTasksService.GetAll();
            FixedTasks = _fixedTasksService.GetAll();
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
                    Priority = inputDynamicTaskModel.Priority,
                    MinTimeToFinish = inputDynamicTaskModel.MinTimeToFinish,
                    MaxTimeToFinish = inputDynamicTaskModel.MaxTimeToFinish,
                    OptimalTimeToFinish = inputDynamicTaskModel.OptimalTimeToFinish
                };
                /*dynamicTask.CategoryDynamicTasks = inputDynamicTaskModel.CategoryIds.Select(cId => new CategoryDynamicTask()
                {
                    DynamicTask = dynamicTask,
                    CategoryId = cId,
                }).ToList();
                */
                if (!dynamicTask.IsObjectValid(out var validationResults))
                {
                    foreach (var validationResult in validationResults)
                    {
                        ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage ?? "");
                    }

                    return Page();
                }

                await _dynamicTasksService.UpdateAsync(dynamicTask);
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
                    Priority = inputFixedTaskModel.Priority,
                    StartTimestamp = DateTime.ParseExact(inputFixedTaskModel.StartTimestamp, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                    EndTimestamp = DateTime.ParseExact(inputFixedTaskModel.EndTimestamp, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)
                };
                /*fixedTask.CategoryFixedTasks = inputFixedTaskModel.CategoryIds.Select(cId => new CategoryFixedTask()
                {
                    FixedTask = fixedTask,
                    CategoryId = cId,
                }).ToList();
                */
                if (!fixedTask.IsObjectValid(out var validationResults))
                {
                    foreach (var validationResult in validationResults)
                    {
                        ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage ?? "");
                    }

                    return Page();
                }

                await _fixedTasksService.UpdateAsync(fixedTask);
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
