using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.UseCases.Organizations.Queries;
using Absence.Application.Common.Interfaces;
using AutoMapper;
using Absence.Domain.Repositories;

namespace Absence.Application.UseCases.Organizations.Handlers;

public class GetOrganizationMembersHandler(
    IOrganizationUsersRepository organizationUserRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<GetOrganizationMembersQuery, OneOf<Success<IEnumerable<MemberDTO>>, BadRequest>>
{
    public async Task<OneOf<Success<IEnumerable<MemberDTO>>, BadRequest>> Handle(GetOrganizationMembersQuery request, CancellationToken cancellationToken)
    {
        var organizationUser = await organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.OrganizationId} found.");
        }

        var organizationUsers = await organizationUserRepository.GetAsync(
            [
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ],
            cancellationToken
        );

        return new Success<IEnumerable<MemberDTO>>(mapper.Map<IEnumerable<MemberDTO>>(organizationUsers));
    }
}