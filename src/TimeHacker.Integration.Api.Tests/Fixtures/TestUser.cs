using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity.Data;
using TimeHacker.Api.Converters.Input.Tasks.RepeatingEntities;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

internal static class TestUser
{
    // Password satisfies the Identity policy (digit, upper, lower, len>=6).
    public static (string Email, string Password) New()
        => ($"user-{Guid.NewGuid():N}@test.local", "Passw0rd!");
}

internal static class ApiClientExtensions
{
    // Mirrors the API's own JSON setup (AddControllers().AddJsonOptions) so request bodies — including the
    // polymorphic RepeatingEntity models — are serialized the way the controllers expect to read them.
    internal static readonly JsonSerializerOptions JsonOptions = BuildJsonOptions();
    private static JsonSerializerOptions BuildJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new InputRepeatingEntityTypeConverter());
        return options;
    }

    // TestServer has no real port; requests use relative URIs resolved against the client BaseAddress.
    // new Uri(string) assumes an absolute URI and throws on a path, so build relative URIs explicitly.
    internal static Uri GetUri(string relativeUrl) => new(relativeUrl, UriKind.Relative);

    public static Task<HttpResponseMessage> RegisterAsync(this HttpClient client, string email, string password)
        => client.PostAsJsonAsync(GetUri("/register"), new RegisterRequest { Email = email, Password = password });

    public static Task<HttpResponseMessage> LoginAsync(this HttpClient client, string email, string password)
        => client.PostAsJsonAsync(GetUri("/login?useCookies=true"), new LoginRequest { Email = email, Password = password });

    public static async Task LoadCsrfTokenAsync(this HttpClient client)
    {
        var token = await client.GetFromJsonAsync<CsrfToken>(GetUri("/api/antiforgery/token"));
        client.DefaultRequestHeaders.Remove("X-XSRF-TOKEN");
        client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token!.Token);
    }

    // Send a real Input DTO (InputCategoryModel, InputFixedTaskModel, ...) using the API's serializer.
    public static Task<HttpResponseMessage> PostDtoAsync<TDto>(this HttpClient client, string url, TDto dto, CancellationToken cancellationToken = default)
        => client.PostAsJsonAsync(GetUri(url), dto, JsonOptions, cancellationToken);

    public static Task<HttpResponseMessage> PutDtoAsync<TDto>(this HttpClient client, string url, TDto dto, CancellationToken cancellationToken = default)
        => client.PutAsJsonAsync(GetUri(url), dto, JsonOptions, cancellationToken);

    public static Task<TValue?> ReadJsonAsync<TValue>(this HttpResponseMessage response, CancellationToken cancellationToken = default)
        => response.Content.ReadFromJsonAsync<TValue>(JsonOptions, cancellationToken);

    private sealed record CsrfToken(string Token);
}
