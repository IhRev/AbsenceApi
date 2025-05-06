using Absence.Application.UseCases.Organizations.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.Mappings;

internal class OrganizationMapping : Profile
{
    public OrganizationMapping()
    {
        CreateMap<CreateOrganizationDTO, OrganizationEntity>();
    }
}