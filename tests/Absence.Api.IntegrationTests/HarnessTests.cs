using System.Net;

namespace Absence.Api.IntegrationTests;

[Collection(nameof(AbsenceApiCollection))]
public class HarnessTests(AbsenceApiFactory factory)
{
    [Fact]
    public async Task Registers_logs_in_and_resolves_the_current_user()
    {
        var user = await factory.RegisterAsync("Alice", "Owner");

        Assert.True(user.ShortId > 0, "ShortId should be assigned by the database.");
    }

    [Fact]
    public async Task Rejects_an_anonymous_request()
    {
        using var anonymous = factory.CreateClient();

        var response = await anonymous.GetAsync("/users/details");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Creates_an_organization_whose_creator_is_owner_and_admin()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");

        var organizationId = await owner.CreateOrganizationAsync();

        Assert.True(organizationId > 0);

        var members = await owner.Client.GetAsync($"/organizations/{organizationId}/members");
        Assert.Equal(HttpStatusCode.OK, members.StatusCode);
    }
}
