using Absence.Application.Common.Abstractions;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Entities;
using MediatR;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class DeleteAbsenceHandler(IRepository<AbsenceEntity> absenceRepository) : IRequestHandler<DeleteAbsenceCommand>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;

    public Task Handle(DeleteAbsenceCommand request, CancellationToken cancellationToken) =>
        _absenceRepository.DeleteByIdAsync(request.Id);
}