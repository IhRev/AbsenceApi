using Absence.Application.UseCases.Absences.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.Mappings;

internal class AbsenceMapping : Profile
{
    public AbsenceMapping()
    {
        CreateMap<CreateAbsenceDTO, AbsenceEntity>()
            .ForMember(dest => dest.AbsenceTypeId, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<AbsenceEntity, AbsenceDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.AbsenceTypeId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.AbsenceStatusId));
    }
}