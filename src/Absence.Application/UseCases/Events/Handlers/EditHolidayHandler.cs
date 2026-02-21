using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class EditHolidayHandler(
    IRepository<EventEntity> holidayRepository,
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<EditEventCommand, OneOf<Success, NotFound, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(EditEventCommand request, CancellationToken cancellationToken)
    {
        var holiday = await holidayRepository.GetByIdAsync(request.Holiday.Id);
        if (holiday is null)
        {
            return new NotFound();
        }

        if (!await userOrganizationRoleRepository.HasPermission(holiday.OrganizationId, user.ShortId, Permissions.MANAGE_EVENTS, cancellationToken))
        {
            return new AccessDenied();
        }

        holiday = mapper.Map(request.Holiday, holiday);
        holidayRepository.Update(holiday);
        await holidayRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}