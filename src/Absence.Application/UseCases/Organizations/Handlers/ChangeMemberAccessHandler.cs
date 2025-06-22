using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Domain.Interfaces;

namespace Absence.Application.UseCases.Organizations.Handlers;

public class ChangeMemberAccessHandler(
    IUser user,
    IOrganizationUsersRepository organizationUsersRepository
) : IRequestHandler<ChangeMemberAccessCommand, OneOf<Success, NotFound, AccessDenied, BadRequest>>
{
    private readonly IUser _user = user;
    private readonly IOrganizationUsersRepository _organizationUsersRepository = organizationUsersRepository;

    public async Task<OneOf<Success, NotFound, AccessDenied, BadRequest>> Handle(ChangeMemberAccessCommand request, CancellationToken cancellationToken)
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
        if (organizationOwner.Organization.OwnerId != _user.ShortId)
        {
            return new AccessDenied();
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