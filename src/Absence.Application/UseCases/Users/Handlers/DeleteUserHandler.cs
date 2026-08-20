using Absence.Application.UseCases.Users.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.Identity;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;

namespace Absence.Application.UseCases.Users.Handlers;

internal class DeleteUserHandler(
    IUserService userService,
    IUser user,
    IRepository<OrganizationEntity> organizationRepository
) : IRequestHandler<DeleteUserCommand, OneOf<Success, BadRequest>>
{
    private readonly IUserService _userService = userService;
    private readonly IUser _user = user;
    private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;

    public async Task<OneOf<Success, BadRequest>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);

        if (!await _userService.CheckPasswordAsync(user!, request.Request.Password))
        {
            return new BadRequest("Password is invalid.");
        }

        var ownedOrganization = await _organizationRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.OwnerId == _user.ShortId)
            ],
            cancellationToken
        );
        if (ownedOrganization is not null)
        {
            return new BadRequest("Transfer or delete owned organizations first.");
        }

        await _userService.DeleteAsync(user!);

        return new Success();
    }
}
