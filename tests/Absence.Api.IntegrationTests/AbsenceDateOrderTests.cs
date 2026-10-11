using System.Net;
using System.Net.Http.Json;

namespace Absence.Api.IntegrationTests;

/// <summary>
/// An inverted absence range is rejected before the handler runs. The message is the same one the
/// accept path still returns when a stored request has its dates out of order.
/// </summary>
[Collection(nameof(AbsenceApiCollection))]
public class AbsenceDateOrderTests(AbsenceApiFactory factory)
{
    private static readonly DateTimeOffset Start = new(2034, 6, 4, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End = new(2034, 6, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task An_inverted_range_is_rejected_on_create_and_on_update()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var typeId = (await owner.Client.GetFromJsonAsync<List<TypePayload>>(
            $"/organizations/{organizationId}/absences/types"))!
            .Single(type => type.Name == "Vacation").Id;

        var created = await owner.Client.PostAsJsonAsync("/absences", new
        {
            name = "Backwards",
            type = typeId,
            startDate = Start,
            endDate = End,
            organization = organizationId
        });

        var absenceId = await owner.CreateAbsenceAsync(organizationId, typeId, End, Start);
        var edited = await owner.Client.PutAsJsonAsync("/absences", new
        {
            id = absenceId,
            name = "Backwards",
            type = typeId,
            startDate = Start,
            endDate = End
        });

        Assert.Equal(HttpStatusCode.BadRequest, created.StatusCode);
        Assert.Contains("Start date must be before end date.", await created.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.BadRequest, edited.StatusCode);
        Assert.Contains("Start date must be before end date.", await edited.Content.ReadAsStringAsync());
    }

    private sealed record TypePayload(int Id, string Name);
}
