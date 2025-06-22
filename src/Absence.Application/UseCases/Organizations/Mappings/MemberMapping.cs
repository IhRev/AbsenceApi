using Absence.Application.UseCases.Organizations.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Organizations.Mappings;

internal class MemberMapping : Profile
{
    public MemberMapping()
    {
        CreateMap<OrganizationUserEntity, MemberDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))
            .ForMember(dest => dest.IsOwner, opt => opt.MapFrom(src => src.UserId == src.Organization.OwnerId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));
    }
}