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

internal class AddHolidayHandler(
    IRepository<EventEntity> holidayRepository,
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<AddEventCommand, OneOf<Success<int>, AccessDenied>>
{
    public async Task<OneOf<Success<int>, AccessDenied>> Handle(AddEventCommand request, CancellationToken cancellationToken)
    {
        if (!await userOrganizationRoleRepository.HasPermission(request.Holiday.OrganizationId, user.ShortId, Permissions.MANAGE_EVENTS, cancellationToken))
        {
            return new AccessDenied();
        }

        var holiday = mapper.Map<EventEntity>(request.Holiday);
        await holidayRepository.InsertAsync(holiday, cancellationToken);
        await holidayRepository.SaveAsync(cancellationToken);
        return new Success<int>(holiday.Id);
    }
}