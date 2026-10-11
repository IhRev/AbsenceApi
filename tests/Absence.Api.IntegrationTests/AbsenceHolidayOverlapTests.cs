using System.Net;
using System.Net.Http.Json;
using Absence.Api.Common;

namespace Absence.Api.IntegrationTests;

public class AbsenceHolidayOverlapTests
{
    private static readonly DateTimeOffset Start = new(2035, 3, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End = new(2035, 3, 4, 0, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(3, 0, 0)]
    [InlineData(1, 12, 0)]
    public void A_holiday_on_either_end_or_between_them_overlaps(int dayOffset, int hour, int minute)
    {
        var holiday = Start.AddDays(dayOffset).AddHours(hour).AddMinutes(minute);

        Assert.True(AbsenceHolidayOverlap.Contains(Start, End, holiday));
    }

    [Fact]
    public void A_holiday_at_the_single_instant_of_a_same_day_absence_overlaps()
    {
        Assert.True(AbsenceHolidayOverlap.Contains(Start, Start, Start));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void A_holiday_outside_the_absence_does_not_overlap(int tickDirection)
    {
        var holiday = tickDirection < 0 ? Start.AddTicks(-1) : End.AddTicks(1);

        Assert.False(AbsenceHolidayOverlap.Contains(Start, End, holiday));
    }
}

[Collection(nameof(AbsenceApiCollection))]
public class AbsenceHolidayOverlapEndpointTests(AbsenceApiFactory factory)
{
    [Fact]
    public async Task A_holiday_on_the_last_instant_blocks_the_absence_and_that_absence_blocks_the_holiday()
    {
        var owner = await factory.RegisterAsync("Olivia", "Owner");
        var organizationId = await owner.CreateOrganizationAsync();
        var typeId = (await owner.Client.GetFromJsonAsync<List<TypePayload>>(
            $"/organizations/{organizationId}/absences/types"))!
            .Single(type => type.Name == "Vacation").Id;
        var start = new DateTimeOffset(2036, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2036, 1, 4, 0, 0, 0, TimeSpan.Zero);

        await owner.CreateHolidayAsync(organizationId, end);
        var blockedAbsence = await owner.Client.PostAsJsonAsync("/absences", new
        {
            name = "Trip",
            type = typeId,
            startDate = start,
            endDate = end,
            organization = organizationId
        });

        var laterStart = end.AddDays(2);
        var laterEnd = end.AddDays(5);
        await owner.CreateAbsenceAsync(organizationId, typeId, laterStart, laterEnd);
        var blockedHoliday = await owner.Client.PostAsJsonAsync("/holidays", new
        {
            name = "Bridge",
            date = laterEnd,
            organizationId
        });
        var afterTheAbsence = await owner.Client.PostAsJsonAsync("/holidays", new
        {
            name = "After",
            date = laterEnd.AddTicks(1),
            organizationId
        });

        Assert.Equal(HttpStatusCode.BadRequest, blockedAbsence.StatusCode);
        Assert.Contains("Absence overlaps a holiday.", await blockedAbsence.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.BadRequest, blockedHoliday.StatusCode);
        Assert.Contains("Holiday overlaps an existing absence.", await blockedHoliday.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, afterTheAbsence.StatusCode);
    }

    private sealed record TypePayload(int Id, string Name);
}
