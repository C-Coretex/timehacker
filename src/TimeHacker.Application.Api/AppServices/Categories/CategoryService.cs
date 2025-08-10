using Microsoft.EntityFrameworkCore;
using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Contracts.IAppServices.Categories;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.IRepositories.Categories;

namespace TimeHacker.Application.Api.AppServices.Categories
{
    public class CategoryService(ICategoryRepository categoryRepository)
        : ICategoryAppService
    {
        public IAsyncEnumerable<CategoryDto> GetAll() => categoryRepository.GetAll(true).Select(CategoryDto.Selector).AsAsyncEnumerable();

        public Task AddAsync(CategoryDto categoryDto)
        {
            return categoryRepository.AddAndSaveAsync(categoryDto.GetEntity(new Category()));
        }

        public async Task UpdateAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new NotProvidedException(nameof(categoryDto));

            var entity = await categoryRepository.GetByIdAsync(categoryDto.Id!.Value);
            await categoryRepository.UpdateAndSaveAsync(categoryDto.GetEntity(entity));
        }

        public Task DeleteAsync(Guid id)
        {
            return categoryRepository.DeleteAndSaveAsync(id);
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var entity = await categoryRepository.GetAll(true).FirstAsync(x => x.Id == id);
            return CategoryDto.Create(entity);
        }
    }
}
