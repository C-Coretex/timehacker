using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.IServices.Categories;

namespace TimeHacker.Application.Pages
{
    [Authorize]
    public class CategoriesModel : PageModel
    {
        private readonly ILogger<CategoriesModel> _logger;
        private readonly string _userId;
        private ICategoryService _categoriesService { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public CategoriesModel(ILogger<CategoriesModel> logger, ICategoryService categoriesService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _categoriesService = categoriesService;
            var user = httpContextAccessor.HttpContext?.User;
            _userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not found");
        }
        
        public void OnGet()
        {
            Categories = _categoriesService.GetAll().ToList();//filter by (EMPTY or user id)
        }
    }
}
