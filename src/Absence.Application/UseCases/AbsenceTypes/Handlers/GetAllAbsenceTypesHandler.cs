using Absence.Application.UseCases.AbsenceTypes.DTOs;
using Absence.Application.UseCases.AbsenceTypes.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

internal class GetAllAbsenceTypesHandler(IRepository<AbsenceTypeEntity> absenceTypeRepository, IMapper mapper)
    : IRequestHandler<GetAllAbsenceTypesQuery, IEnumerable<AbsenceTypeDTO>>
{
    public async Task<IEnumerable<AbsenceTypeDTO>> Handle(
        GetAllAbsenceTypesQuery request, 
        CancellationToken cancellationToken = default
    )
    {
        var absenceTypes = await absenceTypeRepository.GetAsync(
            new AbsenceTypeSpec(request.OrganizationId), 
            cancellationToken
        );
        return mapper.Map<IEnumerable<AbsenceTypeDTO>>(absenceTypes);
    }
}