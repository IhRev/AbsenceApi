using Absence.Domain.Entities;
using Ardalis.Specification;

namespace Absence.Domain.Specifications;

public class InvitationSpec : Specification<OrganizationUserInvitationEntity>
{
    public InvitationSpec(int userId)
    {
        Query
            .Where(invitation => invitation.Invited == userId)
            .Include(_ => _.InviterUser)
            .Include(_ => _.Organization);
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