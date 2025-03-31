using Absence.Application.Common.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.Common.Mappings;

internal class OrganizationMapping : Profile
{
    public OrganizationMapping()
    {
        CreateMap<CreateOrganizationDTO, OrganizationEntity>();
    }
}