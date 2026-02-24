namespace Absence.Application.UseCases.Departments.DTOs;

public class CreateDepartmentDTO
{
    public required string Name { get; set; }
    public int OrganizationId { get; set; }
}