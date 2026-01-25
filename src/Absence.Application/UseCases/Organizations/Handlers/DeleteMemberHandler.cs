using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.Commands;
using Absence.Domain.Repositories;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Handlers;

internal class DeleteMemberHandler(
    IUser user, 
    IOrganizationUsersRepository organizationUsersRepository
) : IRequestHandler<DeleteMemberCommand, OneOf<Success, NotFound, BadRequest, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var organizationOwner = await organizationUsersRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.OrganizationId == request.OrganizationId && _.UserId == user.ShortId)
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

        var organizationUser = await organizationUsersRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.OrganizationId == request.OrganizationId && _.UserId == request.MemberId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"User with id {request.MemberId} doesn't belong to organization.");
        }

        organizationUsersRepository.Delete(organizationUser);
        await organizationUsersRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}