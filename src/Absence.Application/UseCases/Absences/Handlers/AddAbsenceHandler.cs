using Absence.Application.Common.Abstractions;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class AddAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository, 
    IMapper mapper
) : IRequestHandler<AddAbsenceCommand, int>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<int> Handle(AddAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = _mapper.Map<AbsenceEntity>(request.Absence);
        absence.UserId = request.UserId;
        await _absenceRepository.InsertAsync(absence, cancellationToken);
        await _absenceRepository.SaveAsync(cancellationToken);
        return absence.Id;
    }
}