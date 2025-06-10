using Absence.Application.UseCases.Organizations.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Organizations.Mappings;

internal class MemberMapping : Profile
{
    public MemberMapping()
    {
        CreateMap<UserEntity, MemberDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ShortId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
    }
}