using System.ComponentModel.DataAnnotations;

namespace Absence.Application.Common.DTOs;

public class AbsenceDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public int Type { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public int UserId { get; set; }
}