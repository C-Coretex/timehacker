using Microsoft.AspNetCore.Identity;
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

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly bool _isSignedIn;
        private readonly ClaimsPrincipal? _user;
        private readonly IDynamicTaskService _dynamicTasksService;
        private readonly IFixedTaskService _fixedTasksService;

        public enum OpenModalType
        {
            None,
            DynamicTask,
            FixedTask,
        }

        [TempData]
        public int? OpenModal { get; set; }

        public InputDynamicTaskModel InputDynamicTaskModel { get; set; }
        public InputFixedTaskModel InputFixedTaskModel { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            SignInManager<IdentityUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IDynamicTaskService dynamicTasksServiceCommand,
            IFixedTaskService fixedTasksServiceCommand
        )
        {
            _logger = logger;
            _user = httpContextAccessor.HttpContext?.User;
            _isSignedIn = _user != null && signInManager.IsSignedIn(_user);

            _dynamicTasksService = dynamicTasksServiceCommand;
            _fixedTasksService = fixedTasksServiceCommand;
        }

        public IActionResult OnGet()
        {
            if (!_isSignedIn)
                return RedirectToPage("/Landing");

            return Page();
        }


        public async Task<IActionResult> OnPostDynamicTaskFormHandler(InputDynamicTaskModel inputDynamicTaskModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    OpenModal = (int)OpenModalType.DynamicTask;
                    return Page();
                }

                var userId = _user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not found");
                var dynamicTask = new DynamicTask
                {
                    UserId = userId,
                    Name = inputDynamicTaskModel.Name,
                    Description = inputDynamicTaskModel.Description,
                    Priority = inputDynamicTaskModel.Priority,
                    MinTimeToFinish = inputDynamicTaskModel.MinTimeToFinish,
                    MaxTimeToFinish = inputDynamicTaskModel.MaxTimeToFinish,
                    OptimalTimeToFinish = inputDynamicTaskModel.OptimalTimeToFinish,
                };
                /*dynamicTask.CategoryDynamicTasks = inputDynamicTaskModel.CategoryIds.Select(cId => new CategoryDynamicTask()
                {
                    DynamicTask = dynamicTask,
                    CategoryId = cId,
                }).ToList();*/

                if (!dynamicTask.IsObjectValid(out var validationResults))
                {
                    foreach (var validationResult in validationResults)
                    {
                        ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage ?? "");
                    }

                    return Page();
                }

                await _dynamicTasksService.AddAsync(dynamicTask);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding dynamic task");
                throw;
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostFixedTaskFormHandler(InputFixedTaskModel inputFixedTaskModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    OpenModal = (int)OpenModalType.FixedTask;
                    return Page();
                }

                var userId = _user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not found");
                var fixedTask = new FixedTask
                {
                    UserId = userId,
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

                await _fixedTasksService.AddAsync(fixedTask);
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