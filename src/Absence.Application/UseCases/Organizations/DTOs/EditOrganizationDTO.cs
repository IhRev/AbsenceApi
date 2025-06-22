using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Organizations.DTOs;

public class EditOrganizationDTO
{
    [Required]
    public int Id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
}