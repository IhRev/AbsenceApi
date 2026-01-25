using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

internal class CreateAbsenceTypeHandler(
    IOrganizationUsersRepository organizationUsersRepository,
    IRepository<AbsenceTypeEntity> absenceTypesRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<CreateAbsenceTypeCommand, OneOf<Success<int>, BadRequest>>
{
    public async Task<OneOf<Success<int>, BadRequest>> Handle(CreateAbsenceTypeCommand request, CancellationToken cancellationToken)
    {
        var organizationUser = await organizationUsersRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.OrganizationId} found.");
        }

        var absenceType = mapper.Map<AbsenceTypeEntity>(request.AbsenceType);
        absenceType.OrganizationId = request.OrganizationId;
        await absenceTypesRepository.InsertAsync(absenceType, cancellationToken);
        await absenceTypesRepository.SaveAsync(cancellationToken);

        return new Success<int>(absenceType.Id);
    }
}