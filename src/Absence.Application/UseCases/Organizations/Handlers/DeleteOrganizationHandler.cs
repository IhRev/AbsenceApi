using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.Identity;
using Absence.Application.UseCases.Organizations.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Handlers;

public class DeleteOrganizationHandler(
    IUser user,
    IRepository<OrganizationEntity> organizationRepository,
    IUserService userService
) : IRequestHandler<DeleteOrganizationCommand, OneOf<Success, AccessDenied, NotFound>>
{
    private readonly IUser _user = user;
    private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;
    private readonly IUserService _userService = userService;

    public async Task<OneOf<Success, AccessDenied, NotFound>> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);
        if (!await _userService.CheckPasswordAsync(user!, request.Request.Password))
        {
            return new AccessDenied();
        }

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