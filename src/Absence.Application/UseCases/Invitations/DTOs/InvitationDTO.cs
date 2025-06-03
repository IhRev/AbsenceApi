namespace Absence.Application.UseCases.Invitations.DTOs;

public class InvitationDTO
{
    public int Id { get; set; }
    public required string Organization { get; set; }
    public required string Inviter { get; set; }
}