using System.Net;

namespace Absence.Api.IntegrationTests;

/// <summary>
/// Locks in the access policy: a caller who is not a member of the organization gets 404 so that
/// organization ids are not confirmed to strangers, while a member lacking the required right gets 403.
///
/// Only endpoints that run the access gate before loading any other resource belong in these
/// theories. Endpoints that look up a holiday or absence first would answer 404 because that
/// resource is missing, which would pass for the wrong reason; those are covered separately in
/// <see cref="ResourceScopedAccessTests"/>.
/// </summary>
[Collection(nameof(AbsenceApiCollection))]
public class OrganizationAccessTests(AbsenceApiFactory factory)
{
    private static readonly DateTimeOffset From = DateTimeOffset.UtcNow.AddDays(-1);
    private static readonly DateTimeOffset To = DateTimeOffset.UtcNow.AddDays(60);

    public static TheoryData<string, string, string?> MemberReadableEndpoints => new()
    {
        { "GET", "/organizations/{org}/holidays?startDate={from}&endDate={to}", null },
        { "GET", "/organizations/{org}/members", null },
        { "GET", "/organizations/{org}/absences?startDate={from}&endDate={to}", null },
        { "GET", "/organizations/{org}/absences/types", null }
    };

    public static TheoryData<string, string, string?> AdminOnlyEndpoints => new()
    {
        { "POST", "/holidays", """{"name":"Founders Day","date":"2030-01-01T00:00:00+00:00","organizationId":{org}}""" },
        { "GET", "/organizations/{org}/absences/events", null },
        { "POST", "/invitations", """{"userEmail":"nobody@test.local","organizationId":{org}}""" },
        { "POST", "/organizations/{org}/absences?startDate={from}&endDate={to}", "[1]" },
        { "PUT", "/organizations", """{"id":{org},"name":"Renamed"}""" },
        { "DELETE", "/organizations/{org}", """{"password":"Passw0rd!"}""" },
        { "PUT", "/organizations/{org}/members/{member}?isAdmin=true", null },
        { "DELETE", "/organizations/{org}/members/{member}", null },
        { "POST", "/organizations/{org}/absences/types", """{"name":"Unpaid"}""" },
        { "PUT", "/organizations/{org}/absences/types/1", """{"name":"Renamed"}""" },
        { "DELETE", "/organizations/{org}/absences/types/1", null }
    };

    public static TheoryData<string, string, string?> AllGatedEndpoints
    {
        get
        {
            var all = new TheoryData<string, string, string?>();
            foreach (var row in MemberReadableEndpoints.Concat(AdminOnlyEndpoints))
            {
                all.Add((string)row[0]!, (string)row[1]!, (string?)row[2]);
            }

            return all;
        }
    }

    [Theory]
    [MemberData(nameof(AllGatedEndpoints))]
    public async Task A_non_member_gets_404(string method, string path, string? json)
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var outsider = await factory.RegisterAsync("Oscar", "Outsider");

        var response = await outsider.SendAsync(
            method,
            Fill(path, organizationId, owner.ShortId),
            FillJson(json, organizationId, owner.ShortId));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(AdminOnlyEndpoints))]
    public async Task A_member_without_the_right_gets_403(string method, string path, string? json)
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var member = await factory.AddMemberAsync(owner, organizationId);

        var response = await member.SendAsync(
            method,
            Fill(path, organizationId, member.ShortId),
            FillJson(json, organizationId, member.ShortId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(MemberReadableEndpoints))]
    public async Task A_member_can_read(string method, string path, string? json)
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var member = await factory.AddMemberAsync(owner, organizationId);

        var response = await member.SendAsync(
            method,
            Fill(path, organizationId, member.ShortId),
            FillJson(json, organizationId, member.ShortId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task An_organization_that_does_not_exist_is_404_even_for_a_valid_user()
    {
        var user = await factory.RegisterAsync("Nora", "Nobody");

        var response = await user.Client.GetAsync(
            $"/organizations/999999/holidays?startDate={From.Encode()}&endDate={To.Encode()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Deleting_an_organization_checks_access_before_the_password()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var outsider = await factory.RegisterAsync("Oscar", "Outsider");

        // A wrong password must not turn into 403, which would reveal that the organization exists.
        var response = await outsider.SendAsync(
            "DELETE",
            $"/organizations/{organizationId}",
            """{"password":"definitely-not-the-password"}""");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task An_admin_who_is_not_the_owner_cannot_edit_or_delete_the_organization()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var admin = await factory.AddMemberAsync(owner, organizationId, "Adam");
        await owner.PromoteToAdminAsync(organizationId, admin.ShortId);

        var edit = await admin.SendAsync("PUT", "/organizations", $$"""{"id":{{organizationId}},"name":"Renamed"}""");
        var delete = await admin.SendAsync("DELETE", $"/organizations/{organizationId}", """{"password":"Passw0rd!"}""");

        Assert.Equal(HttpStatusCode.Forbidden, edit.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, delete.StatusCode);
    }

    private static string Fill(string template, int organizationId, int memberShortId) =>
        template
            .Replace("{org}", organizationId.ToString())
            .Replace("{member}", memberShortId.ToString())
            .Replace("{from}", From.Encode())
            .Replace("{to}", To.Encode());

    private static string? FillJson(string? template, int organizationId, int memberShortId) =>
        template is null ? null : Fill(template, organizationId, memberShortId);
}
