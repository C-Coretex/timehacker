using Microsoft.AspNetCore.Identity.Data;
using Refit;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

/// <summary>Identity + antiforgery endpoints used to establish an authenticated session.</summary>
public interface IAuthApi
{
    // Register/Login return plain Task so Refit throws on setup failure.
    [Post("/register")]
    Task Register([Body] RegisterRequest request);

    [Post("/login?useCookies=true")]
    Task Login([Body] LoginRequest request);

    [Get("/api/antiforgery/token")]
    Task<IApiResponse<CsrfToken>> GetAntiforgeryToken();
}

public sealed record CsrfToken(string Token);
