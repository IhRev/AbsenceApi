using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Absences.DTOs;
using Absence.Application.UseCases.Absences.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class GetUserAbsencesHandler(
    IRepository<AbsenceEntity> absenceRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<GetUserAbsencesQuery, IEnumerable<AbsenceDTO>>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<IEnumerable<AbsenceDTO>> Handle(GetUserAbsencesQuery request, CancellationToken cancellationToken)
    {
        var absences = await _absenceRepository.GetAsync(
            [ 
                q => q.Where(_ => _.UserId == _user.Id),
                q => q.Where(_ => _.StartDate < request.EndDate && _.EndDate > request.StartDate)
            ], 
            cancellationToken
        );
        return _mapper.Map<IEnumerable<AbsenceDTO>>(absences);
    }
}