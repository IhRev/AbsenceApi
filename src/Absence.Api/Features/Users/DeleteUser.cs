using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using Absence.Infrastructure.Identity;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Users;

public class DeleteUserRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string Password { get; set; }
}

public static class DeleteUser
{
    public sealed class Command(DeleteUserRequest request) : IRequest<OneOf<Success, BadRequest>>
    {
        public DeleteUserRequest Request { get; } = request;
    }

    internal sealed class Handler(
        IUserService userService,
        IUser user,
        IRepository<OrganizationEntity> organizationRepository
    ) : IRequestHandler<Command, OneOf<Success, BadRequest>>
    {
        private readonly IUserService _userService = userService;
        private readonly IUser _user = user;
        private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;

        public async Task<OneOf<Success, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
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
}
