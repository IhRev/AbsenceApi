using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using Absence.Application.UseCases.Absences.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class GetUsersAbsencesHandler(
    IRepository<AbsenceEntity> absenceRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<GetUsersAbsencesQuery, OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>>
{
    public async Task<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>> Handle(GetUsersAbsencesQuery request, CancellationToken cancellationToken)
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

        var absences = await absenceRepository.GetAsync(
            [
                q => q.Where(_ => request.UserIds.Contains(_.UserId)),
                q => q.Where(_ => _.StartDate < request.EndDate && _.EndDate > request.StartDate),
                q => q.Where(_ => _.OrganizationId == request.OrganizationId),
            ],
            cancellationToken
        );
        return new Success<IEnumerable<AbsenceDTO>>(mapper.Map<IEnumerable<AbsenceDTO>>(absences));
    }
}