using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using Absence.Infrastructure.Identity;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public class DeleteOrganizationRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string Password { get; set; }
}

public static class DeleteOrganization
{
    public sealed class Command(int id, DeleteOrganizationRequest request) : IRequest<OneOf<Success, NotFound, AccessDenied>>
    {
        public int Id { get; } = id;
        public DeleteOrganizationRequest Request { get; } = request;
    }

    internal sealed class Handler(
        IUser user,
        IRepository<OrganizationEntity> organizationRepository,
        IUserService userService
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied>>
    {
        private readonly IUser _user = user;
        private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;
        private readonly IUserService _userService = userService;

        public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
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
}
