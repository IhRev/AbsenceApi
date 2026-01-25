using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using Absence.Application.UseCases.AbsenceTypes.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

internal class GetAllAbsenceTypesHandler(
    IRepository<AbsenceTypeEntity> absenceTypeRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<GetAllAbsenceTypesQuery, OneOf<Success<IEnumerable<AbsenceTypeDTO>>, BadRequest>>
{
    public async Task<OneOf<Success<IEnumerable<AbsenceTypeDTO>>, BadRequest>> Handle(GetAllAbsenceTypesQuery request, CancellationToken cancellationToken = default)
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

        var absenceTypes = await absenceTypeRepository.GetAsync(
            [ 
                q => q.Where(_ => _.OrganizationId == request.OrganizationId) 
            ], 
            cancellationToken
        );
        return new Success<IEnumerable<AbsenceTypeDTO>>(mapper.Map<IEnumerable<AbsenceTypeDTO>>(absenceTypes));
    }
}