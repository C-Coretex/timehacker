using TimeHacker.Api.Models.Input.Categories;
using TimeHacker.Api.Models.Return.Categories;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

/// <summary>Typed surface over <c>CategoriesController</c> (/api/categories).</summary>
public interface ICategoriesApi
{
    [Get("/api/categories")]
    Task<IApiResponse<IReadOnlyList<CategoryReturnModel>>> GetAll();

    [Get("/api/categories/{id}")]
    Task<IApiResponse<CategoryReturnModel>> Get(Guid id);

    [Post("/api/categories")]
    Task<IApiResponse<Guid>> Create([Body] InputCategoryModel model);

    [Put("/api/categories/{id}")]
    Task<IApiResponse> Update(Guid id, [Body] InputCategoryModel model);

    [Delete("/api/categories/{id}")]
    Task<IApiResponse> Delete(Guid id);
}
