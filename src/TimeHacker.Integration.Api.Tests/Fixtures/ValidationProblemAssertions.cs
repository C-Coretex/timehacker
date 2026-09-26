using System.Text.Json;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

// Pins the ValidationProblemDetails body [ApiController] returns when a data annotation rejects one field.
internal static class ValidationProblemAssertions
{
    public static void ShouldBeValidationErrorFor(this IApiResponse response, string field)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.GetProperty("title").GetString().Should().Be("One or more validation errors occurred.");
        body.RootElement.GetProperty("errors").TryGetProperty(field, out var messages).Should().BeTrue();
        messages.EnumerateArray().Should().NotBeEmpty();
    }
}
