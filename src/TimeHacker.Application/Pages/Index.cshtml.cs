using Helpers.Domain.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Security.Claims;
using TimeHacker.Application.Models.PageModels;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Pages
{

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly bool _isSignedIn;
        private readonly ClaimsPrincipal? _user;
        private readonly IDynamicTasksServiceCommand _dynamicTasksServiceCommand;
        private readonly IFixedTasksServiceCommand _fixedTasksServiceCommand;

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
            IDynamicTasksServiceCommand dynamicTasksServiceCommand,
            IFixedTasksServiceCommand fixedTasksServiceCommand)
        {
            _logger = logger;
            _user = httpContextAccessor.HttpContext?.User;
            _isSignedIn = _user != null && signInManager.IsSignedIn(_user);

            _dynamicTasksServiceCommand = dynamicTasksServiceCommand;
            _fixedTasksServiceCommand = fixedTasksServiceCommand;
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

                await _dynamicTasksServiceCommand.AddAsync(dynamicTask);
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

                await _fixedTasksServiceCommand.AddAsync(fixedTask);
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