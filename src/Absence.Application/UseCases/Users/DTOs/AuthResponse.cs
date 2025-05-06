namespace Absence.Application.UseCases.Users.DTOs;

public class AuthResponse
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public string? AccessToken { get; }
    public string? RefreshToken { get; }

    private AuthResponse(bool success, string? message = null, string? accessToken = null, string? refreshToken = null)
    {
        IsSuccess = success;
        Message = message;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public static AuthResponse Success(string accessToken, string refreshToken) => 
        new AuthResponse(true, accessToken: accessToken, refreshToken: refreshToken);

    public static AuthResponse Fail(string message) =>
        new AuthResponse(false, message: message);
}