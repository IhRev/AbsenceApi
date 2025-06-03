using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class DeleteAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository, 
    IUser user
) : IRequestHandler<DeleteAbsenceCommand, OneOf<Success, NotFound, AccessDenied>>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IUser _user = user;

    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(DeleteAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await _absenceRepository.GetByIdAsync(request.Id);
        if (absence is null)
        {
            return new NotFound();
        }
        if (absence.UserId != _user.ShortId)
        {
            return new AccessDenied();
        }

        _absenceRepository.Delete(absence);
        await _absenceRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}