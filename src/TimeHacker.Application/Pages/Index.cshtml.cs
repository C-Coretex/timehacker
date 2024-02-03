using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Tasks;

namespace TimeHacker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly bool _isSignedIn;
        private readonly ClaimsPrincipal? _user;
        private readonly IDynamicTasksServiceCommand _dynamicTasksServiceCommand;
        private readonly IFixedTasksServiceCommand _fixedTasksServiceCommand;

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


        public async Task<IActionResult> OnPostDynamicTaskFormHandler(
            string name, 
            string description, 
            string category, 
            uint priority, 
            TimeSpan minTimeToFinish, 
            TimeSpan maxTimeToFinish,
            TimeSpan optimalTimeToFinish)
        {
            var userId = _user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not found");
            var dynamicTask = new DynamicTask
            {
                UserId = userId,
                Name = name,
                Description = description,
                Category = category,
                Priority = priority,
                MinTimeToFinish = minTimeToFinish,
                MaxTimeToFinish = maxTimeToFinish,
                OptimalTimeToFinish = optimalTimeToFinish
            };

            await _dynamicTasksServiceCommand.AddAsync(dynamicTask);

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostFixedTaskFormHandler(
            string name,
            string description,
            string category,
            uint priority,
            DateTime startTimestamp,
            DateTime endTimestamp)
        {
            var userId = _user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not found");
            var fixedTask = new FixedTask
            {
                UserId = userId,
                Name = name,
                Description = description,
                Category = category,
                Priority = priority,
                StartTimestamp = startTimestamp,
                EndTimestamp = endTimestamp
            };

            await _fixedTasksServiceCommand.AddAsync(fixedTask);

            return RedirectToPage();
            //return Page();
        }
    }
}