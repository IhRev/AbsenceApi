using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class AddAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository, 
    IMapper mapper,
    IUser user
) : IRequestHandler<AddAbsenceCommand, int>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<int> Handle(AddAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = _mapper.Map<AbsenceEntity>(request.Absence);
        absence.UserId = _user.Id;
        await _absenceRepository.InsertAsync(absence, cancellationToken);
        await _absenceRepository.SaveAsync(cancellationToken);
        return absence.Id;
    }
}