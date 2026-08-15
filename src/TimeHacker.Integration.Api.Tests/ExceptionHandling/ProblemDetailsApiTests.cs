using System.Text.Json;

namespace TimeHacker.Integration.Api.Tests.ExceptionHandling;

// Pins the response-body contract produced by LogExceptionFilter (ProblemDetails) and the framework's
// automatic model-validation (ValidationProblemDetails). Status codes themselves are covered per-controller.
public sealed class ProblemDetailsApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    [Fact, Trait("Exception", "NotFoundException -> 404 ProblemDetails")]
    public async Task NotFound_Should_ReturnProblemDetailsBody()
    {
        var api = await CreateAuthenticatedApiAsync();

        // POST /api/tasks/schedules throws NotFoundException for an unknown parent.
        var response = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(Guid.CreateVersion7(), TestRequests.EveryNDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.GetProperty("title").GetString().Should().Be("Resource is not found.");
        body.RootElement.TryGetProperty("ResourceName", out _).Should().BeTrue();
    }

    [Fact, Trait("Exception", "GET-by-id -> 404 ProblemDetails")]
    public async Task GetByIdNotFound_Should_ReturnProblemDetailsBody()
    {
        var api = await CreateAuthenticatedApiAsync();

        // GET-by-id 404s go through the same NotFoundException path, so they carry a ProblemDetails body too.
        var response = await api.Categories.Get(Guid.CreateVersion7());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.GetProperty("title").GetString().Should().Be("Resource is not found.");
    }

    [Fact, Trait("Exception", "DataIsNotCorrectException -> 400 ProblemDetails")]
    public async Task BadData_Should_ReturnProblemDetailsBody()
    {
        var api = await CreateAuthenticatedApiAsync();
        var start = new DateTime(2026, 07, 01, 10, 00, 00, DateTimeKind.Utc);
        var end = new DateTime(2026, 07, 01, 09, 00, 00, DateTimeKind.Utc); // end before start

        var response = await api.FixedTasks.Create(TestRequests.NewFixedTask("Bad", start, end));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.GetProperty("title").GetString().Should().Be("Parameter is not correct.");
    }

    [Fact, Trait("Exception", "Model validation -> 400 ValidationProblemDetails")]
    public async Task ModelValidation_Should_ReturnValidationProblemDetailsBody()
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Categories.Create(TestRequests.NewCategory(name: ""));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.TryGetProperty("errors", out var errors).Should().BeTrue();
        errors.EnumerateObject().Should().NotBeEmpty();
    }
}
