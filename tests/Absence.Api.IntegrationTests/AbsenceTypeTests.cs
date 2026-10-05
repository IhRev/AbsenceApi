using System.Net;
using System.Net.Http.Json;

namespace Absence.Api.IntegrationTests;

/// <summary>
/// Absence types belong to one organization. A new organization starts with Vacation and Sick leave,
/// admins can add, rename and delete types, and a type stays while an absence or a pending request uses it.
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

    [Fact]
    public async Task An_admin_can_add_a_type_and_another_organization_can_use_the_same_name()
    {
        var first = await factory.RegisterAsync("Olivia", "Owner");
        var firstOrganizationId = await first.CreateOrganizationAsync();
        var second = await factory.RegisterAsync("Sam", "Owner");
        var secondOrganizationId = await second.CreateOrganizationAsync();

        var created = await first.Client.PostAsJsonAsync(
            $"/organizations/{firstOrganizationId}/absences/types",
            new { name = "Unpaid" });
        var duplicateInOtherOrganization = await second.Client.PostAsJsonAsync(
            $"/organizations/{secondOrganizationId}/absences/types",
            new { name = "Unpaid" });

        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        Assert.Equal(HttpStatusCode.OK, duplicateInOtherOrganization.StatusCode);
        Assert.Contains("Unpaid", (await TypesAsync(first, firstOrganizationId)).Select(_ => _.Name));
        Assert.Contains("Unpaid", (await TypesAsync(second, secondOrganizationId)).Select(_ => _.Name));
    }

    [Fact]
    public async Task A_duplicate_name_in_the_same_organization_is_rejected()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();

        var response = await owner.Client.PostAsJsonAsync(
            $"/organizations/{organizationId}/absences/types",
            new { name = "Vacation" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(2, (await TypesAsync(owner, organizationId)).Count);
    }

    [Fact]
    public async Task An_admin_can_rename_a_type()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var vacationId = (await TypesAsync(owner, organizationId)).Single(_ => _.Name == "Vacation").Id;

        var sameName = await owner.Client.PutAsJsonAsync(
            $"/organizations/{organizationId}/absences/types/{vacationId}",
            new { name = "Vacation" });
        var renamed = await owner.Client.PutAsJsonAsync(
            $"/organizations/{organizationId}/absences/types/{vacationId}",
            new { name = "Annual leave" });

        Assert.Equal(HttpStatusCode.OK, sameName.StatusCode);
        Assert.Equal(HttpStatusCode.OK, renamed.StatusCode);
        Assert.Contains("Annual leave", (await TypesAsync(owner, organizationId)).Select(_ => _.Name));
        Assert.DoesNotContain("Vacation", (await TypesAsync(owner, organizationId)).Select(_ => _.Name));
    }

    [Fact]
    public async Task Renaming_onto_an_existing_name_is_rejected()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var vacationId = (await TypesAsync(owner, organizationId)).Single(_ => _.Name == "Vacation").Id;

        var response = await owner.Client.PutAsJsonAsync(
            $"/organizations/{organizationId}/absences/types/{vacationId}",
            new { name = "Sick leave" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Vacation", (await TypesAsync(owner, organizationId)).Select(_ => _.Name));
    }

    [Fact]
    public async Task An_unused_type_can_be_deleted()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var created = await owner.Client.PostAsJsonAsync(
            $"/organizations/{organizationId}/absences/types",
            new { name = "Unpaid" });
        created.EnsureSuccessStatusCode();
        var typeId = await created.Content.ReadFromJsonAsync<int>();

        var response = await owner.Client.DeleteAsync($"/organizations/{organizationId}/absences/types/{typeId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(typeId, (await TypesAsync(owner, organizationId)).Select(_ => _.Id));
    }

    [Fact]
    public async Task A_type_used_by_an_absence_cannot_be_deleted()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var vacationId = (await TypesAsync(owner, organizationId)).Single(_ => _.Name == "Vacation").Id;
        await owner.CreateAbsenceAsync(
            organizationId,
            vacationId,
            new DateTimeOffset(2031, 6, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2031, 6, 5, 0, 0, 0, TimeSpan.Zero));

        var response = await owner.Client.DeleteAsync($"/organizations/{organizationId}/absences/types/{vacationId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(vacationId, (await TypesAsync(owner, organizationId)).Select(_ => _.Id));
    }

    [Fact]
    public async Task A_type_referenced_by_a_pending_request_cannot_be_deleted()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var member = await factory.AddMemberAsync(owner, organizationId);
        var vacationId = (await TypesAsync(owner, organizationId)).Single(_ => _.Name == "Vacation").Id;
        var requested = await member.Client.PostAsJsonAsync("/absences", new
        {
            name = "Trip",
            type = vacationId,
            startDate = new DateTimeOffset(2031, 7, 1, 0, 0, 0, TimeSpan.Zero),
            endDate = new DateTimeOffset(2031, 7, 3, 0, 0, 0, TimeSpan.Zero),
            organization = organizationId
        });
        requested.EnsureSuccessStatusCode();

        var response = await owner.Client.DeleteAsync($"/organizations/{organizationId}/absences/types/{vacationId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(vacationId, (await TypesAsync(owner, organizationId)).Select(_ => _.Id));
    }

    [Fact]
    public async Task A_type_cannot_be_changed_through_another_organizations_route()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var other = await factory.RegisterAsync("Sam", "Owner");
        var otherOrganizationId = await other.CreateOrganizationAsync();
        var vacationId = (await TypesAsync(owner, organizationId)).Single(_ => _.Name == "Vacation").Id;

        var rename = await other.Client.PutAsJsonAsync(
            $"/organizations/{otherOrganizationId}/absences/types/{vacationId}",
            new { name = "Hijacked" });
        var delete = await other.Client.DeleteAsync(
            $"/organizations/{otherOrganizationId}/absences/types/{vacationId}");

        Assert.Equal(HttpStatusCode.NotFound, rename.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
        Assert.Contains("Vacation", (await TypesAsync(owner, organizationId)).Select(_ => _.Name));
    }

    private static async Task<List<AbsenceTypePayload>> TypesAsync(TestUser user, int organizationId) =>
        await user.Client.GetFromJsonAsync<List<AbsenceTypePayload>>($"/organizations/{organizationId}/absences/types")
        ?? throw new InvalidOperationException("Absence type list came back null.");

    private sealed record AbsenceTypePayload(int Id, string Name);
}
