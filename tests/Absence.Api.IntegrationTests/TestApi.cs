using System.Net.Http.Headers;
using System.Net.Http.Json;

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
}
