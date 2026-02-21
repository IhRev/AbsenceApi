using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class DeleteHolidayHandler(
    IRepository<EventEntity> holidayRepository,
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IUser user
) : IRequestHandler<DeleteEventCommand, OneOf<Success, NotFound, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var holiday = await holidayRepository.GetByIdAsync(request.Id);
        if (holiday is null)
        {
            return new NotFound();
        }

        if (!await userOrganizationRoleRepository.HasPermission(holiday.OrganizationId, user.ShortId, Permissions.MANAGE_EVENTS, cancellationToken))
        {
            return new AccessDenied();
        }

        holidayRepository.Delete(holiday);
        await holidayRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}