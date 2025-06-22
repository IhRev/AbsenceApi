using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.UseCases.Organizations.Queries;
using Absence.Domain.Interfaces;
using Absence.Application.Common.Interfaces;
using AutoMapper;

namespace Absence.Application.UseCases.Organizations.Handlers;

public class GetOrganizationMembersHandler(
    IOrganizationUsersRepository organizationUserRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<GetOrganizationMembersQuery, OneOf<Success<IEnumerable<MemberDTO>>, BadRequest, AccessDenied>>
{
    private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
    private readonly IUser _user = user;
    private readonly IMapper _mapper = mapper;
    
    public async Task<OneOf<Success<IEnumerable<MemberDTO>>, BadRequest, AccessDenied>> Handle(GetOrganizationMembersQuery request, CancellationToken cancellationToken)
    {
        var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == _user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.OrganizationId} found.");
        }
        if (!organizationUser.IsAdmin)
        {
            return new AccessDenied();
        }

        var organizationUsers = await _organizationUserRepository.GetAsync(
            [
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ],
            cancellationToken
        );

        return new Success<IEnumerable<MemberDTO>>(_mapper.Map<IEnumerable<MemberDTO>>(organizationUsers));
    }
}