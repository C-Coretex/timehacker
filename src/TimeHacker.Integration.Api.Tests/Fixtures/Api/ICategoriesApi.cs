using TimeHacker.Api.Models.Input.Categories;
using TimeHacker.Api.Models.Return.Categories;
using TimeHacker.Api.Models.Return.ScheduleSnapshots;

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

    /// <summary>Attaches a recurrence. ParentEntityId is a <b>category schedule</b> id, not a category id.</summary>
    [Post("/api/categories/schedules")]
    Task<IApiResponse<ScheduleEntityReturnModel>> CreateRecurrence([Body] InputScheduleEntityModel model);

    [Get("/api/categories/{categoryId}/schedules")]
    Task<IApiResponse<IReadOnlyList<CategoryScheduleReturnModel>>> GetSchedules(Guid categoryId);

    [Post("/api/categories/{categoryId}/schedules")]
    Task<IApiResponse<Guid>> CreateSchedule(Guid categoryId, [Body] InputCategoryScheduleModel model);

    [Put("/api/categories/{categoryId}/schedules/{id}")]
    Task<IApiResponse> UpdateSchedule(Guid categoryId, Guid id, [Body] InputCategoryScheduleModel model);

    [Delete("/api/categories/{categoryId}/schedules/{id}")]
    Task<IApiResponse> DeleteSchedule(Guid categoryId, Guid id);
}
