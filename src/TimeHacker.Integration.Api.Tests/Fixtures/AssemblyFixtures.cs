using TimeHacker.Integration.Api.Tests.Fixtures;

[assembly: AssemblyFixture(typeof(ApiTestFixture))]
// Disable parallel because Respawner is being called in the end of each test to clean up the database
// If we need to enable parallel test execution, we would have to make sure parallel tests use different users to not interfere
[assembly: CollectionBehavior(DisableTestParallelization = true)]