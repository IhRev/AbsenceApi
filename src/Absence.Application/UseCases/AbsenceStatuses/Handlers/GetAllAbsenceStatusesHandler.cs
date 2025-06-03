using Absence.Application.UseCases.AbsenceStatuses.DTOs;
using Absence.Application.UseCases.AbsenceStatuses.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.AbsenceStatuses.Handlers;

internal class GetAllAbsenceStatusesHandler(IRepository<AbsenceStatusEntity> absenceStatusRepository, IMapper mapper) : IRequestHandler<GetAllAbsenceStatusesQuery, IEnumerable<AbsenceStatusDTO>>
{
    private readonly IRepository<AbsenceStatusEntity> _absenceStatusRepository = absenceStatusRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<AbsenceStatusDTO>> Handle(GetAllAbsenceStatusesQuery request, CancellationToken cancellationToken = default)
    {
        var absenceStatuses = await _absenceStatusRepository.GetAsync(cancellationToken: cancellationToken);
        return _mapper.Map<IEnumerable<AbsenceStatusDTO>>(absenceStatuses);
    }
}