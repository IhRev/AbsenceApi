using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using Absence.Application.UseCases.Absences.Queries;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using AutoMapper;
using Absence.Domain.Repositories;

namespace Absence.Application.UseCases.Absences.Handlers;

public class GetAbsenceEventsHandler(
    IOrganizationUsersRepository organizationUserRepository,
    IAbsenceEventRepository absenceEventRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<GetAbsenceEventsQuery, OneOf<Success<IEnumerable<AbsenceEventDTO>>, BadRequest, AccessDenied>>
{
    public async Task<OneOf<Success<IEnumerable<AbsenceEventDTO>>, BadRequest, AccessDenied>> Handle(GetAbsenceEventsQuery request, CancellationToken cancellationToken)
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
        if (!organizationUser.IsAdmin)
        {
            return new AccessDenied();
        }

        var events = await absenceEventRepository.GetAsync(
            [
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ]
        );
        return new Success<IEnumerable<AbsenceEventDTO>>(mapper.Map<IEnumerable<AbsenceEventDTO>>(events));
    }
}