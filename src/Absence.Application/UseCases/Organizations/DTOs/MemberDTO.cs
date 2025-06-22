namespace Absence.Application.UseCases.Organizations.DTOs;

public class MemberDTO
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public bool IsOwner { get; set; }
    public bool IsAdmin { get; set; }
}