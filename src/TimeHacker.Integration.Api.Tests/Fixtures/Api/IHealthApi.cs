using Refit;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

/// <summary>The infrastructure health endpoint.</summary>
public interface IHealthApi
{
    [Get("/health")]
    Task<IApiResponse<string>> Get();
}
