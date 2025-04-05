using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class DeleteAbsenceHandler(IRepository<AbsenceEntity> absenceRepository) : IRequestHandler<DeleteAbsenceCommand>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;

    public async Task Handle(DeleteAbsenceCommand request, CancellationToken cancellationToken)
    {
        await _absenceRepository.DeleteByIdAsync(request.Id, cancellationToken);
        await _absenceRepository.SaveAsync(cancellationToken);
    }
}