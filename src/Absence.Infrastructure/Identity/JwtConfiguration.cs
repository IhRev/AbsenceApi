namespace Absence.Infrastructure.Identity;

public class JwtConfiguration
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Secret { get; set; }
    public int JwtTokenExpireTimeInMinutes { get; set; }
    public int RefreshTokenExpireTimeInDays { get; set; }
}