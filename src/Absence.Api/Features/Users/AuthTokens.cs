namespace Absence.Api.Features.Users;

public class AuthTokens
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
