using Refit;
using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.Tasks;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

/// <summary>Typed surface over <c>FixedTasksController</c> (/api/fixed-tasks).</summary>
public interface IFixedTasksApi
{
    [Get("/api/fixed-tasks")]
    Task<IApiResponse<IReadOnlyList<FixedTaskReturnModel>>> GetAll();

    [Get("/api/fixed-tasks/{id}")]
    Task<IApiResponse<FixedTaskReturnModel>> Get(Guid id);

    [Post("/api/fixed-tasks")]
    Task<IApiResponse<Guid>> Create([Body] InputFixedTaskModel model);

    [Put("/api/fixed-tasks/{id}")]
    Task<IApiResponse> Update(Guid id, [Body] InputFixedTaskModel model);

    [Delete("/api/fixed-tasks/{id}")]
    Task<IApiResponse> Delete(Guid id);
}
