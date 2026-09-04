using Absence.Infrastructure.Entities;
using AutoMapper;

namespace Absence.Api.Features.Invitations;

internal class InvitationMapping : Profile
{
    public InvitationMapping()
    {
        CreateMap<OrganizationUserInvitationEntity, InvitationDTO>()
            .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.Organization.Name))
            .ForMember(dest => dest.Inviter, opt => opt.MapFrom(src => src.InviterUser.Email));

        CreateMap<OrganizationUserInvitationEntity, OrganizationUserEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Invited))
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => false));
    }
}
