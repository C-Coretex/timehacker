namespace TimeHacker.Integration.Api.Tests.Infrastructure;

public sealed class HealthApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    [Fact, Trait("Endpoint", "GET /health")]
    public async Task Health_Should_Return200_WhenDatabasesReachable()
    {
        var api = CreateAnonymousApi(); // /health needs no auth

        var response = await api.Health.Get();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact, Trait("Endpoint", "GET /health")]
    public async Task Health_Should_Return503_WhenDatabaseUnreachable()
    {
        var api = new TimeHackerApi(Fixture.CreateBrokenDbApiClient());

        var response = await api.Health.Get();

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }
}
