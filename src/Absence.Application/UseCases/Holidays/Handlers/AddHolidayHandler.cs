using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Holidays.Handlers;

internal class AddHolidayHandler(
    IRepository<HolidayEntity> holidayRepository,
    IMapper mapper,
    IUser user,
    IRepository<OrganizationUserEntity> organizationUserRepository
) : IRequestHandler<AddHolidayCommand, OneOf<Success<int>, BadRequest, AccessDenied>>
{
    private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
    private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<OneOf<Success<int>, BadRequest, AccessDenied>> Handle(AddHolidayCommand request, CancellationToken cancellationToken)
    {
        var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
           [
               q => q.Where(_ => _.UserId == _user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.Holiday.OrganizationId)
           ],
           cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.Holiday.OrganizationId} found.");
        }
        if (!organizationUser.IsAdmin)
        {
            return new AccessDenied();
        }

        var holiday = _mapper.Map<HolidayEntity>(request.Holiday);
        await _holidayRepository.InsertAsync(holiday, cancellationToken);
        await _holidayRepository.SaveAsync(cancellationToken);
        return new Success<int>(holiday.Id);
    }
}