using System.Net;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Integration.Api.Tests.Fixtures;

namespace TimeHacker.Integration.Api.Tests.Users;

public sealed class UsersApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    [Fact, Trait("Endpoint", "GET /api/users/me")]
    public async Task GetCurrent_Should_ReturnAutoProvisionedProfile()
    {
        var api = await CreateAuthenticatedApiAsync();

        var me = await api.Users.GetCurrent();

        me.StatusCode.Should().Be(HttpStatusCode.OK);
        me.Content.Should().NotBeNull();
    }

    [Fact, Trait("Endpoint", "PUT /api/users/me")]
    public async Task Update_Should_PersistProfileChanges()
    {
        var api = await CreateAuthenticatedApiAsync();

        var update = await api.Users.UpdateCurrent(TestRequests.NewUserUpdate(
            name: "Jane Doe", phone: "+15551234567", email: "jane@notify.local"));
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var me = await api.Users.GetCurrent();
        me.Content!.Name.Should().Be("Jane Doe");
        me.Content.EmailForNotifications.Should().Be("jane@notify.local");
        me.Content.PhoneNumberForNotifications.Should().Be("+15551234567");
    }

    [Theory, Trait("Endpoint", "Validation")]
    [InlineData("", "+15551234567", "ok@notify.local")]        // empty name
    [InlineData("Valid", "not-a-phone", "ok@notify.local")]    // bad phone
    [InlineData("Valid", "+15551234567", "not-an-email")]      // bad email
    public async Task Update_Should_Return400_ForInvalidInput(string name, string phone, string email)
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Users.UpdateCurrent(TestRequests.NewUserUpdate(name, phone, email));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Endpoint", "DELETE /api/users/me")]
    public async Task Delete_Should_Return204_AndCascadeUsersData()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        await api.Categories.Create(TestRequests.NewCategory("Cat"));
        await api.FixedTasks.Create(TestRequests.NewFixedTask("Task"));

        var delete = await api.Users.DeleteCurrent();
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await AdminDbContext.Set<User>().CountAsync(cancellationToken)).Should().Be(0);
        (await AdminDbContext.Set<Category>().CountAsync(cancellationToken)).Should().Be(0);
        (await AdminDbContext.Set<FixedTask>().CountAsync(cancellationToken)).Should().Be(0);
    }
}
