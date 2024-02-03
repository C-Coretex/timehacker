using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TimeHacker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly bool _isSignedIn;

        public IndexModel(ILogger<IndexModel> logger, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            var user = httpContextAccessor.HttpContext?.User;
            _isSignedIn = user != null && signInManager.IsSignedIn(user);
        }

        public IActionResult OnGet()
        {
            if (!_isSignedIn)
                return RedirectToPage("/Landing");

            return Page();
        }

        // Handle Form 1 submission
        public IActionResult OnPostForm1Handler()
        {
            // Logic for handling Form 1 submission
            return RedirectToPage();
        }

        // Handle Form 2 submission
        public IActionResult OnPostForm2Handler()
        {
            // Logic for handling Form 2 submission
            return Page();
        }
    }
}