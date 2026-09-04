using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public static class ChangeMemberAccess
{
    public sealed class Command(int organizationId, int userId, bool isAdmin) : IRequest<OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        public int OrganizationId { get; } = organizationId;
        public int UserId { get; } = userId;
        public bool IsAdmin { get; } = isAdmin;
    }

    internal sealed class Handler(
        IUser user,
        IOrganizationUsersRepository organizationUsersRepository,
        IRepository<OrganizationEntity> organizationRepository
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        private readonly IUser _user = user;
        private readonly IOrganizationUsersRepository _organizationUsersRepository = organizationUsersRepository;
        private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;

        public async Task<OneOf<Success, NotFound, AccessDenied, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var organizationOwner = await _organizationUsersRepository.GetFirstOrDefaultAsync(
                [
                    q => q.Where(_ => _.OrganizationId == request.OrganizationId && _.UserId == _user.ShortId)
                ],
                cancellationToken
            );
            if (organizationOwner is null)
            {
                return new NotFound();
            }
            if (!organizationOwner.IsAdmin)
            {
                return new AccessDenied();
            }

            var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
            if (organization is null)
            {
                return new NotFound();
            }

            var organizationUser = await _organizationUsersRepository.GetFirstOrDefaultAsync(
                [
                    q => q.Where(_ => _.OrganizationId == request.OrganizationId && _.UserId == request.UserId)
                ],
                cancellationToken
            );
            if (organizationUser is null)
            {
                return new BadRequest($"User with id {request.UserId} doesn't belong to organization.");
            }
            if (organizationUser.UserId == organization.OwnerId)
            {
                return new BadRequest("Cannot change the organization owner's access.");
            }

            if (organizationUser.IsAdmin == request.IsAdmin)
            {
                return new BadRequest("Cannot change access to the same.");
            } 

            organizationUser.IsAdmin = request.IsAdmin;
            _organizationUsersRepository.Update(organizationUser);
            await _organizationUsersRepository.SaveAsync(cancellationToken);

            return new Success();
        }
    }
}
