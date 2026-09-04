using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public static class GetOrganizationMembers
{
    public sealed class Query(int organizationId) : IRequest<OneOf<Success<IEnumerable<MemberDTO>>, BadRequest>>
    {
        public int OrganizationId { get; } = organizationId;
    }

    internal sealed class Handler(
        IOrganizationUsersRepository organizationUserRepository,
        IUser user,
        IMapper mapper
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<MemberDTO>>, BadRequest>>
    {
        private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
        private readonly IUser _user = user;
        private readonly IMapper _mapper = mapper;

        public async Task<OneOf<Success<IEnumerable<MemberDTO>>, BadRequest>> Handle(Query request, CancellationToken cancellationToken)
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

            var organizationUsers = await _organizationUserRepository.GetAsync(
                [
                    q => q.Where(_ => _.OrganizationId == request.OrganizationId)
                ],
                cancellationToken
            );

            return new Success<IEnumerable<MemberDTO>>(_mapper.Map<IEnumerable<MemberDTO>>(organizationUsers));
        }
    }
}
