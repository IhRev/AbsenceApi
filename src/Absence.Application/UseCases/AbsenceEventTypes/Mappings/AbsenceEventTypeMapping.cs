using Absence.Application.UseCases.AbsenceEventTypes.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.AbsenceEventTypes.Mappings;

internal class AbsenceEventTypeMapping : Profile
{
    public AbsenceEventTypeMapping()
    {
        CreateMap<AbsenceEventTypeEntity, AbsenceEventTypeDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()));
    }
}