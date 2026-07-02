using Refit;
using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.Tasks;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

/// <summary>Typed surface over <c>DynamicTasksController</c> (/api/dynamic-tasks).</summary>
public interface IDynamicTasksApi
{
    [Get("/api/dynamic-tasks")]
    Task<IApiResponse<IReadOnlyList<DynamicTaskReturnModel>>> GetAll();

    [Get("/api/dynamic-tasks/{id}")]
    Task<IApiResponse<DynamicTaskReturnModel>> Get(Guid id);

    [Post("/api/dynamic-tasks")]
    Task<IApiResponse<Guid>> Create([Body] InputDynamicTaskModel model);

    [Put("/api/dynamic-tasks/{id}")]
    Task<IApiResponse> Update(Guid id, [Body] InputDynamicTaskModel model);

    [Delete("/api/dynamic-tasks/{id}")]
    Task<IApiResponse> Delete(Guid id);
}
