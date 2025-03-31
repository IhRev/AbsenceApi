using Absence.Application.Common.Abstractions;
using Absence.Application.UseCases.Organizations.Commands;
using Absence.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Organizations.Handlers;

internal class AddOrganizationHandler(
    IRepository<OrganizationEntity> organizationRepository,
    IMapper mapper
) : IRequestHandler<AddOrganizationCommand, int>
{
    private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<int> Handle(AddOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = _mapper.Map<OrganizationEntity>(request.Organization);
        await _organizationRepository.InsertAsync(organization, cancellationToken);
        await _organizationRepository.SaveAsync(cancellationToken);
        return organization.Id;
    }
}