using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Organizations.DTOs;

public class DeleteOrganizationRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string Password { get; set; }
}