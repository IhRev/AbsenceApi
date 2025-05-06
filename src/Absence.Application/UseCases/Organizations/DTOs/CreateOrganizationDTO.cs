using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Organizations.DTOs;

public class CreateOrganizationDTO
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = null!;
}