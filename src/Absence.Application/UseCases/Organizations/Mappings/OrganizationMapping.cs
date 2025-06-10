using Absence.Application.UseCases.Organizations.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Organizations.Mappings;

internal class OrganizationMapping : Profile
{
    public OrganizationMapping()
    {
        CreateMap<CreateOrganizationDTO, OrganizationEntity>()
            .ForMember(dest => dest.Id, src => src.Ignore())
            .ForMember(dest => dest.OwnerId, src => src.Ignore());

        CreateMap<OrganizationUserEntity, OrganizationDTO>()
            .ForMember(dest => dest.Name, src => src.MapFrom(opt => opt.Organization.Name))
            .ForMember(dest => dest.Id, src => src.MapFrom(opt => opt.OrganizationId))
            .ForMember(
                dest => dest.IsOwner, 
                src => src.MapFrom((src, dest, destMember, context) =>
                    int.Parse(context.Items["UserId"]!.ToString()!) == src.Organization.OwnerId
                )
            );
    }
}