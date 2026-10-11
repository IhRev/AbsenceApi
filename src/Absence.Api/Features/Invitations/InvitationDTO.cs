namespace Absence.Api.Features.Invitations;

public class InvitationDTO
{
    public int Id { get; set; }
    public required string Organization { get; set; }
    public required string Inviter { get; set; }
}
