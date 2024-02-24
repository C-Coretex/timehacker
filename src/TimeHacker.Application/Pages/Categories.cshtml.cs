using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Categories;
using TimeHacker.Domain.Models.Persistence.Categories;

namespace TimeHacker.Application.Pages
{
    public class CategoriesModel : PageModel
    {
        public IEnumerable<Category> Categories { get; set; }

        private CategoriesService _categoriesService {  get; set; }
        public CategoriesModel(CategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }
        
        public void OnGet()
        {
            Categories = _categoriesService.Queries.GetAll().ToList();//filter by (EMPTY or user id)
        }
    }
}
