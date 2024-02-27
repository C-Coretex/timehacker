using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Categories;
using TimeHacker.Domain.Models.Persistence.Categories;

namespace TimeHacker.Application.Pages
{
    [Authorize]
    public class CategoriesModel : PageModel
    {
        private readonly ILogger<CategoriesModel> _logger;
        private readonly string _userId;
        private CategoriesService _categoriesService { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public CategoriesModel(ILogger<CategoriesModel> logger, CategoriesService categoriesService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _categoriesService = categoriesService;
            var user = httpContextAccessor.HttpContext?.User;
            _userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not found");
        }
        
        public void OnGet()
        {
            Categories = _categoriesService.Queries.GetCategoriesForUserId(_userId).ToList();//filter by (EMPTY or user id)
        }
    }
}
