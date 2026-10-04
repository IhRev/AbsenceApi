using System.Net;
using System.Net.Http.Json;

namespace Absence.Api.IntegrationTests;

/// <summary>
/// Absence types belong to one organization and are created with it, so a brand new organization
/// must already have its starter set and that set must not be readable by anyone outside it.
/// </summary>
[Collection(nameof(AbsenceApiCollection))]
public class AbsenceTypeTests(AbsenceApiFactory factory)
{
    [Fact]
    public async Task A_new_organization_starts_with_vacation_and_sick_leave()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();

        var types = await owner.Client.GetFromJsonAsync<List<AbsenceTypePayload>>(
            $"/organizations/{organizationId}/absences/types");

        Assert.NotNull(types);
        Assert.Equal(["Sick leave", "Vacation"], types.Select(_ => _.Name).Order());
    }

    [Fact]
    public async Task A_non_member_cannot_read_another_organizations_absence_types()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var outsider = await factory.RegisterAsync("Oscar", "Outsider");

        var response = await outsider.SendAsync("GET", $"/organizations/{organizationId}/absences/types");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record AbsenceTypePayload(int Id, string Name);
}
