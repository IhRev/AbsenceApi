using System.Net;
using System.Net.Http.Json;

namespace Absence.Api.IntegrationTests;

/// <summary>
/// An admin's absence write is stored immediately and a member's is queued. The status code is what
/// tells those apart: 201, 200 and 204 for an immediate create, update and delete, and 202 when the
/// write is waiting for approval.
/// </summary>
[Collection(nameof(AbsenceApiCollection))]
public class AbsenceWriteResponseTests(AbsenceApiFactory factory)
{
    private static readonly DateTimeOffset Start = new(2032, 4, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End = new(2032, 4, 4, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task An_admin_create_returns_201_and_an_update_or_delete_returns_200_or_204()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var typeId = (await TypesAsync(owner, organizationId)).Single(_ => _.Name == "Vacation").Id;

        var created = await owner.Client.PostAsJsonAsync("/absences", Body(organizationId, typeId, "Trip"));
        var absenceId = await created.Content.ReadFromJsonAsync<int>();

        var edited = await owner.Client.PutAsJsonAsync("/absences", new
        {
            id = absenceId,
            name = "Trip extended",
            type = typeId,
            startDate = Start,
            endDate = End
        });
        var deleted = await owner.Client.DeleteAsync($"/absences/{absenceId}");

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.Equal($"/absences/{absenceId}", created.Headers.Location?.OriginalString);
        Assert.Equal(HttpStatusCode.OK, edited.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
    }

    [Fact]
    public async Task A_member_write_returns_202_until_an_admin_approves_it()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var member = await factory.AddMemberAsync(owner, organizationId);
        var typeId = (await TypesAsync(owner, organizationId)).Single(_ => _.Name == "Vacation").Id;
        var name = $"m-{Guid.NewGuid():N}"[..12];

        var requested = await member.Client.PostAsJsonAsync("/absences", Body(organizationId, typeId, name));

        var events = await owner.Client.GetFromJsonAsync<List<EventPayload>>(
            $"/organizations/{organizationId}/absences/events");
        var accept = await owner.Client.PostAsync(
            $"/absences/events/{events!.Single(_ => _.Name == name).Id}?accepted=true",
            null);
        accept.EnsureSuccessStatusCode();

        var absences = await member.Client.GetFromJsonAsync<List<AbsencePayload>>(
            $"/organizations/{organizationId}/absences?startDate={Start.Encode()}&endDate={End.AddDays(1).Encode()}");
        var absenceId = absences!.Single(_ => _.Name == name).Id;

        var edited = await member.Client.PutAsJsonAsync("/absences", new
        {
            id = absenceId,
            name = name,
            type = typeId,
            startDate = Start,
            endDate = End
        });
        var deleted = await member.Client.DeleteAsync($"/absences/{absenceId}");

        Assert.Equal(HttpStatusCode.Accepted, requested.StatusCode);
        Assert.Equal(string.Empty, await requested.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.Accepted, edited.StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, deleted.StatusCode);
    }

    private static object Body(int organizationId, int typeId, string name) => new
    {
        name,
        type = typeId,
        startDate = Start,
        endDate = End,
        organization = organizationId
    };

    private static async Task<List<TypePayload>> TypesAsync(TestUser user, int organizationId) =>
        await user.Client.GetFromJsonAsync<List<TypePayload>>($"/organizations/{organizationId}/absences/types")
        ?? throw new InvalidOperationException("Absence type list came back null.");

    private sealed record TypePayload(int Id, string Name);

    private sealed record EventPayload(int Id, string Name);

    private sealed record AbsencePayload(int Id, string Name);
}
