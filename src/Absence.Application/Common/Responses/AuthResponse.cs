namespace Absence.Application.Common.Responses;

public class AuthResponse
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public string? AccessToken { get; }

    private AuthResponse(bool success, string? message = null, string? accessToken = null)
    {
        IsSuccess = success;
        Message = message;
        AccessToken = accessToken;
    }

    public static AuthResponse Success(string accessToken) => 
        new AuthResponse(true, accessToken: accessToken);

    public static AuthResponse Fail(string message) =>
        new AuthResponse(false, message: message);
}