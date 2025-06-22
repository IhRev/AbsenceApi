using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Domain.Interfaces;
using Absence.Domain.Entities;

namespace Absence.Application.UseCases.Organizations.Handlers;

public class DeleteOrganizationHandler(
    IUser user,
    IRepository<OrganizationEntity> organizationRepository
) : IRequestHandler<DeleteOrganizationCommand, OneOf<Success, AccessDenied, NotFound>>
{
    private readonly IUser _user = user;
    private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;

    public async Task<OneOf<Success, AccessDenied, NotFound>> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (organization is null)
        {
            return new NotFound();
        }
        if (organization.OwnerId != _user.ShortId)
        {
            return new AccessDenied();
        }

        _organizationRepository.Delete(organization);
        await _organizationRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}