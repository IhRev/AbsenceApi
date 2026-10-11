using System.Net;

namespace Absence.Api.IntegrationTests;

/// <summary>
/// Endpoints that load a holiday or absence before checking access. A real resource is created
/// first, so a 404 here can only come from the access gate and not from the resource being absent.
/// </summary>
[Collection(nameof(AbsenceApiCollection))]
public class ResourceScopedAccessTests(AbsenceApiFactory factory)
{
    [Fact]
    public async Task A_non_member_gets_404_for_a_holiday_that_really_exists()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var holidayId = await owner.CreateHolidayAsync(organizationId, DateTimeOffset.UtcNow.AddDays(30));
        var outsider = await factory.RegisterAsync("Oscar", "Outsider");

        var edit = await outsider.SendAsync("PUT", "/holidays", $$"""
            {"id":{{holidayId}},"name":"Hijacked","date":"2030-02-02T00:00:00+00:00"}
            """);
        var delete = await outsider.SendAsync("DELETE", $"/holidays/{holidayId}");

        Assert.Equal(HttpStatusCode.NotFound, edit.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
    }

    [Fact]
    public async Task A_member_who_is_not_admin_gets_403_for_a_holiday()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var holidayId = await owner.CreateHolidayAsync(organizationId, DateTimeOffset.UtcNow.AddDays(30));
        var member = await factory.AddMemberAsync(owner, organizationId);

        var edit = await member.SendAsync("PUT", "/holidays", $$"""
            {"id":{{holidayId}},"name":"Hijacked","date":"2030-02-02T00:00:00+00:00"}
            """);
        var delete = await member.SendAsync("DELETE", $"/holidays/{holidayId}");

        Assert.Equal(HttpStatusCode.Forbidden, edit.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, delete.StatusCode);
    }

    [Fact]
    public async Task A_holiday_that_does_not_exist_is_404_for_its_own_admin()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        await owner.CreateOrganizationAsync();

        var delete = await owner.SendAsync("DELETE", "/holidays/999999");

        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
    }

    [Fact]
    public async Task A_non_member_gets_404_for_an_absence_that_really_exists()
    {
        var (_, _, absenceId, absenceTypeId) = await CreateAbsenceAsync();
        var outsider = await factory.RegisterAsync("Oscar", "Outsider");

        var edit = await outsider.SendAsync("PUT", "/absences", EditBody(absenceId, absenceTypeId));
        var delete = await outsider.SendAsync("DELETE", $"/absences/{absenceId}");

        Assert.Equal(HttpStatusCode.NotFound, edit.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
    }

    [Fact]
    public async Task A_member_who_does_not_own_the_absence_gets_403()
    {
        var (owner, organizationId, absenceId, absenceTypeId) = await CreateAbsenceAsync();
        var member = await factory.AddMemberAsync(owner, organizationId);

        var edit = await member.SendAsync("PUT", "/absences", EditBody(absenceId, absenceTypeId));
        var delete = await member.SendAsync("DELETE", $"/absences/{absenceId}");

        Assert.Equal(HttpStatusCode.Forbidden, edit.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, delete.StatusCode);
    }

    private static string EditBody(int absenceId, int absenceTypeId) => $$"""
        {"id":{{absenceId}},"name":"Hijacked","type":{{absenceTypeId}},"startDate":"2031-03-01T00:00:00+00:00","endDate":"2031-03-05T00:00:00+00:00"}
        """;

    private async Task<(TestUser Owner, int OrganizationId, int AbsenceId, int AbsenceTypeId)> CreateAbsenceAsync()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var absenceTypeId = await factory.CreateAbsenceTypeAsync(organizationId, "Unpaid");
        var absenceId = await owner.CreateAbsenceAsync(
            organizationId,
            absenceTypeId,
            new DateTimeOffset(2031, 1, 5, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2031, 1, 9, 0, 0, 0, TimeSpan.Zero));

        return (owner, organizationId, absenceId, absenceTypeId);
    }
}
