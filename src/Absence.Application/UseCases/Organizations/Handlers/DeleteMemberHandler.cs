﻿using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.Commands;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Handlers;

internal class DeleteMemberHandler(
    IUser user, 
    IOrganizationUsersRepository organizationUsersRepository
) : IRequestHandler<DeleteMemberCommand, OneOf<Success, NotFound, BadRequest, AccessDenied>>
{
    private readonly IUser _user = user;
    private readonly IOrganizationUsersRepository _organizationUsersRepository = organizationUsersRepository;

    public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
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

        _organizationUsersRepository.Delete(organizationUser);
        await _organizationUsersRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}