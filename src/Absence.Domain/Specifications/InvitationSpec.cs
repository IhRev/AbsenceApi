using Absence.Domain.Entities;
using Ardalis.Specification;

namespace Absence.Domain.Specifications;

public class InvitationSpec : Specification<OrganizationUserInvitationEntity>
{
    public InvitationSpec(int userId)
    {
        Query
            .Where(invitation => invitation.Invited == userId);
    }

    public InvitationSpec(int userId, int organizationId)
    {
        Query
            .Where(invitation =>
                invitation.Invited == userId &&
                invitation.OrganizationId == organizationId
            );
    }
}