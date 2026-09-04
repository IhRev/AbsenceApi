using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using AutoMapper;
using MediatR;

namespace Absence.Api.Features.Organizations;

public class CreateOrganizationDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
}

public static class AddOrganization
{
    public sealed class Command(CreateOrganizationDTO organization) : IRequest<int>
    {
        public CreateOrganizationDTO Organization { get; } = organization;
    }

    internal sealed class Handler(
        IRepository<OrganizationEntity> organizationRepository,
        IMapper mapper,
        IUser user
    ) : IRequestHandler<Command, int>
    {
        private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUser _user = user;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var organization = _mapper.Map<OrganizationEntity>(request.Organization);
            organization.OwnerId = _user.ShortId;
            organization.OrganizationsUsers.Add(new OrganizationUserEntity()
            {
                IsAdmin = true,
                UserId = _user.ShortId
            });
            await _organizationRepository.InsertAsync(organization, cancellationToken);
            await _organizationRepository.SaveAsync(cancellationToken);

            return organization.Id;
        }
    }
}
