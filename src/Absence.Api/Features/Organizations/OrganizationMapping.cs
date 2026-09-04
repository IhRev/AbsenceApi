using Absence.Infrastructure.Entities;
using AutoMapper;

namespace Absence.Api.Features.Organizations;

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

        CreateMap<OrganizationUserEntity, MemberDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))
            .ForMember(dest => dest.IsOwner, opt => opt.MapFrom(src => src.UserId == src.Organization.OwnerId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));
    }
}
