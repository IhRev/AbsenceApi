using Absence.Application.Common.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.Common.Mappings;

internal class AbsenceMapping : Profile
{
    public AbsenceMapping()
    {
        CreateMap<CreateAbsenceDTO, AbsenceEntity>()
            .ForMember(dest => dest.AbsenceTypeId, opt => opt.MapFrom(src => src.Type));

        CreateMap<AbsenceEntity, AbsenceDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.AbsenceTypeId));
    }
}