using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Absence.Api.IntegrationTests;

/// <summary>
/// Login and refresh return the two tokens and nothing else. A failure is the message on its own,
/// with no success flag and no null token fields.
/// </summary>
[Collection(nameof(AbsenceApiCollection))]
public class AuthTests(AbsenceApiFactory factory)
{
    [Fact]
    public async Task Login_returns_the_two_tokens_and_a_wrong_password_returns_only_the_message()
    {
        var user = await factory.RegisterAsync("Alice", "Owner");
        using var anonymous = factory.CreateClient();

        var login = await anonymous.PostAsJsonAsync("/auth/login", new { email = user.Email, password = TestApi.Password });
        using var success = JsonDocument.Parse(await login.Content.ReadAsStringAsync());

        var wrong = await anonymous.PostAsJsonAsync("/auth/login", new { email = user.Email, password = "not-the-password" });
        var failure = await wrong.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Equal(2, success.RootElement.EnumerateObject().Count());
        Assert.False(string.IsNullOrWhiteSpace(success.RootElement.GetProperty("accessToken").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(success.RootElement.GetProperty("refreshToken").GetString()));

        Assert.Equal(HttpStatusCode.BadRequest, wrong.StatusCode);
        Assert.Contains("Incorrect email or password", failure);
        Assert.DoesNotContain("accessToken", failure);
    }

    [Fact]
    public async Task Refresh_returns_new_tokens_and_an_invalid_token_is_401_with_only_the_message()
    {
        var user = await factory.RegisterAsync("Alice", "Owner");
        using var anonymous = factory.CreateClient();

        var login = await anonymous.PostAsJsonAsync("/auth/login", new { email = user.Email, password = TestApi.Password });
        var tokens = await login.Content.ReadFromJsonAsync<AuthPayload>();

        var refreshed = await anonymous.PostAsJsonAsync("/auth/refresh_token", new
        {
            accessToken = tokens!.AccessToken,
            refreshToken = tokens.RefreshToken
        });
        using var success = JsonDocument.Parse(await refreshed.Content.ReadAsStringAsync());

        var rejected = await anonymous.PostAsJsonAsync("/auth/refresh_token", new
        {
            accessToken = "not-a-token",
            refreshToken = "not-a-token"
        });
        var failure = await rejected.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, refreshed.StatusCode);
        Assert.Equal(2, success.RootElement.EnumerateObject().Count());
        Assert.False(string.IsNullOrWhiteSpace(success.RootElement.GetProperty("accessToken").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(success.RootElement.GetProperty("refreshToken").GetString()));

        Assert.Equal(HttpStatusCode.Unauthorized, rejected.StatusCode);
        Assert.Contains("Token is invalid", failure);
        Assert.DoesNotContain("accessToken", failure);
    }
}
