namespace Absence.Application.UseCases.Departments.DTOs;

public class EditDepartmentDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int OrganizationId { get; set; }
}