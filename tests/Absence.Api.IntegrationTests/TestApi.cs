using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Absence.Api.IntegrationTests;

public sealed record TestUser(HttpClient Client, string Email, int ShortId);

public sealed record AuthPayload(bool IsSuccess, string? Message, string? AccessToken, string? RefreshToken);

public sealed record UserDetailsPayload(int Id, string FirstName, string LastName, string Email);

/// <summary>
/// Drives account setup through the real HTTP endpoints so tests exercise the same
/// registration, login and token path as a client would.
/// </summary>
public static class TestApi
{
    public const string Password = "Passw0rd!";

    public static async Task<TestUser> RegisterAsync(
        this AbsenceApiFactory factory,
        string firstName = "Test",
        string lastName = "User")
    {
        var email = $"{firstName.ToLowerInvariant()}-{Guid.NewGuid():N}@test.local";
        using var anonymous = factory.CreateClient();

        var registration = await anonymous.PostAsJsonAsync("/auth/register", new
        {
            firstName,
            lastName,
            credentials = new { email, password = Password }
        });
        registration.EnsureSuccessStatusCode();

        var client = factory.CreateClient();
        var accessToken = await AuthenticateAsync(anonymous, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var details = await client.GetFromJsonAsync<UserDetailsPayload>("/users/details")
            ?? throw new InvalidOperationException("Could not read the details of the user just registered.");

        return new TestUser(client, email, details.Id);
    }

    public static async Task<string> AuthenticateAsync(HttpClient anonymous, string email)
    {
        var login = await anonymous.PostAsJsonAsync("/auth/login", new { email, password = Password });
        login.EnsureSuccessStatusCode();

        var auth = await login.Content.ReadFromJsonAsync<AuthPayload>();
        return auth?.AccessToken
            ?? throw new InvalidOperationException($"Login for {email} returned no access token.");
    }

    public static async Task<int> CreateOrganizationAsync(this TestUser owner, string name = "Acme")
    {
        var response = await owner.Client.PostAsJsonAsync("/organizations", new { name = $"{name} {Guid.NewGuid():N}" });
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    /// <summary>Registers a user and walks them through invitation and acceptance so they end up a plain member.</summary>
    public static async Task<TestUser> AddMemberAsync(
        this AbsenceApiFactory factory,
        TestUser admin,
        int organizationId,
        string firstName = "Mallory")
    {
        var member = await factory.RegisterAsync(firstName, "Member");

        var invite = await admin.Client.PostAsJsonAsync(
            "/invitations",
            new { userEmail = member.Email, organizationId });
        invite.EnsureSuccessStatusCode();

        var invitations = await member.Client.GetFromJsonAsync<List<InvitationPayload>>("/invitations")
            ?? throw new InvalidOperationException("Invitation list came back null.");

        var accept = await member.Client.PostAsync($"/invitations/{invitations.Single().Id}?accepted=true", null);
        accept.EnsureSuccessStatusCode();

        return member;
    }

    public static async Task PromoteToAdminAsync(this TestUser admin, int organizationId, int memberShortId)
    {
        var response = await admin.Client.PutAsync(
            $"/organizations/{organizationId}/members/{memberShortId}?isAdmin=true",
            null);
        response.EnsureSuccessStatusCode();
    }

    public static Task<HttpResponseMessage> SendAsync(this TestUser user, string method, string path, string? json = null)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), path);
        if (json is not null)
        {
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return user.Client.SendAsync(request);
    }

    public static async Task<int> CreateHolidayAsync(this TestUser admin, int organizationId, DateTimeOffset date)
    {
        var response = await admin.Client.PostAsJsonAsync(
            "/holidays",
            new { name = "Founders Day", date, organizationId });
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    /// <summary>
    /// Inserts an absence type directly. Creating an organization already seeds Vacation and Sick leave,
    /// so this is only for types beyond that starter set.
    /// </summary>
    public static async Task<int> CreateAbsenceTypeAsync(this AbsenceApiFactory factory, int organizationId, string name)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AbsenceContext>();

        var type = new AbsenceTypeEntity { Name = name, OrganizationId = organizationId };
        db.AbsenceTypes.Add(type);
        await db.SaveChangesAsync();

        return type.Id;
    }

    /// <summary>
    /// Creates an absence that is persisted straight away, which only happens for an organization
    /// admin. The response is 201 with the new id as the body.
    /// </summary>
    public static async Task<int> CreateAbsenceAsync(
        this TestUser admin,
        int organizationId,
        int absenceTypeId,
        DateTimeOffset startDate,
        DateTimeOffset endDate)
    {
        var response = await admin.Client.PostAsJsonAsync("/absences", new
        {
            name = "Trip",
            type = absenceTypeId,
            startDate,
            endDate,
            organization = organizationId
        });
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    public static string Encode(this DateTimeOffset moment) => Uri.EscapeDataString(moment.ToString("o"));
}

public sealed record InvitationPayload(int Id, string Organization, string Inviter);
