using Absence.Application.UseCases.Invitations.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Invitations.Mappings;

internal class InvitationMapping : Profile
{
    public InvitationMapping()
    {
        CreateMap<OrganizationUserInvitationEntity, InvitationDTO>()
            .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.Organization.Name))
            .ForMember(dest => dest.Inviter, opt => opt.MapFrom(src => src.User.Email));

        CreateMap<OrganizationUserInvitationEntity, OrganizationUserEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => false));
    }
}