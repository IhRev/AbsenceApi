namespace Absence.Application.UseCases.Organizations.DTOs;

public class OrganizationDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsOwner { get; set; }
}