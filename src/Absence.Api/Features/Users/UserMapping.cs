using Absence.Infrastructure.Entities;
using AutoMapper;

namespace Absence.Api.Features.Users;

internal class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<UserEntity, UserDetails>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ShortId));
    }
}
