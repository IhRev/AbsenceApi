using Absence.Application.Common.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.Common.Mappings;

internal class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<CreateUserDTO, UserEntity>()
            .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.Organization));
    }
}