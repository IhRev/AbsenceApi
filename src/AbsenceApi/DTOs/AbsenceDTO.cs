using System.ComponentModel.DataAnnotations;

namespace AbsenceApi.DTOs;

public class AbsenceDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public AbsenceTypeDTO Type { get; set; } = null!;
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public int UserId { get; set; }
}