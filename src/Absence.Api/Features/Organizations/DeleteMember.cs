using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public static class DeleteMember
{
    public sealed class Command(int organizationId, int memberId) : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public int OrganizationId { get; } = organizationId;
        public int MemberId { get; } = memberId;
    }

    internal sealed class Handler(
        IUser user, 
        IOrganizationUsersRepository organizationUsersRepository,
        IRepository<OrganizationEntity> organizationRepository
    ) : IRequestHandler<Command, OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        private readonly IUser _user = user;
        private readonly IOrganizationUsersRepository _organizationUsersRepository = organizationUsersRepository;
        private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;

        public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
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
                    q => q.Where(_ => _.OrganizationId == request.OrganizationId && _.UserId == request.MemberId)
                ],
                cancellationToken
            );
            if (organizationUser is null)
            {
                return new BadRequest($"User with id {request.MemberId} doesn't belong to organization.");
            }
            if (organizationUser.UserId == organization.OwnerId)
            {
                return new BadRequest("Cannot remove the organization owner.");
            }

            _organizationUsersRepository.Delete(organizationUser);
            await _organizationUsersRepository.SaveAsync(cancellationToken);

            return new Success();
        }
    }
}
