using System.ComponentModel.DataAnnotations;

namespace Absence.Application.Common.DTOs;

public class CreateOrganizationDTO
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = null!;
}