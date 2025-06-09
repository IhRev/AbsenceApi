using Absence.Application.UseCases.AbsenceEventTypes.DTOs;
using Absence.Application.UseCases.AbsenceEventTypes.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.AbsenceEventTypes.Handlers;

public class GetAllAbsenceEventTypesHandler(
    IRepository<AbsenceEventTypeEntity> absenceEventTypesRepository,
    IMapper mapper
) : IRequestHandler<GetAllAbsenceEventTypesQuery, IEnumerable<AbsenceEventTypeDTO>>
{
    private readonly IRepository<AbsenceEventTypeEntity> _absenceEventTypesRepository = absenceEventTypesRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<AbsenceEventTypeDTO>> Handle(GetAllAbsenceEventTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await _absenceEventTypesRepository.GetAsync([], cancellationToken);
        return _mapper.Map<IEnumerable<AbsenceEventTypeDTO>>(types);
    }
}