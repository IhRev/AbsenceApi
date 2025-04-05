namespace Absence.Application.Common.Configurations;

public class JwtConfiguration
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Secret { get; set; }
    public int ExpireTimeInMinutes { get; set; }
}