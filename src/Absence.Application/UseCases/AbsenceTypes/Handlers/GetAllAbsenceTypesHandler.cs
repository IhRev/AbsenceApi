using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.AbsenceTypes.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

internal class GetAllAbsenceTypesHandler(IRepository<AbsenceTypeEntity> absenceTypeRepository, IMapper mapper) : IRequestHandler<GetAllAbsenceTypesQuery, IEnumerable<AbsenceTypeDTO>>
{
    private readonly IRepository<AbsenceTypeEntity> _absenceTypeRepository = absenceTypeRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<AbsenceTypeDTO>> Handle(GetAllAbsenceTypesQuery request, CancellationToken cancellationToken = default)
    {
        var absenceTypes = await _absenceTypeRepository.GetAsync(cancellationToken: cancellationToken);
        return _mapper.Map<IEnumerable<AbsenceTypeDTO>>(absenceTypes);
    }
}