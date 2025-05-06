using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Users.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public required string AccessToken { get; set; }
    [Required]
    public required string RefreshToken { get; set; }
}