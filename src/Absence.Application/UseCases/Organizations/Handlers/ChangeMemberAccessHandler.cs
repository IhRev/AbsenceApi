using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Domain.Repositories;

namespace Absence.Application.UseCases.Organizations.Handlers;

public class ChangeMemberAccessHandler(
    IUser user,
    IOrganizationUsersRepository organizationUsersRepository
) : IRequestHandler<ChangeMemberAccessCommand, OneOf<Success, NotFound, AccessDenied, BadRequest>>
{
    public async Task<OneOf<Success, NotFound, AccessDenied, BadRequest>> Handle(ChangeMemberAccessCommand request, CancellationToken cancellationToken)
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
                q => q.Where(_ => _.OrganizationId == request.OrganizationId && _.UserId == request.UserId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"User with id {request.UserId} doesn't belong to organization.");
        }

        if (organizationUser.IsAdmin == request.IsAdmin)
        {
            return new BadRequest("Cannot change access to the same.");
        } 

        organizationUser.IsAdmin = request.IsAdmin;
        organizationUsersRepository.Update(organizationUser);
        await organizationUsersRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}