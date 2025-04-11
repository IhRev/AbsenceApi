using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class DeleteAbsenceHandler(IRepository<AbsenceEntity> absenceRepository) : IRequestHandler<DeleteAbsenceCommand, OneOf<Success, NotFound>>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;

    public async Task<OneOf<Success, NotFound>> Handle(DeleteAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await _absenceRepository.GetByIdAsync(request.Id);
        if (absence is null)
        {
            return new NotFound();
        }

        _absenceRepository.Delete(absence);
        await _absenceRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}