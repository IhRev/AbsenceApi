using Absence.Application.UseCases.Holidays.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.Mappings;

internal class HolidayMapping : Profile
{
    public HolidayMapping()
    {
        CreateMap<HolidayEntity, HolidayDTO>();

        CreateMap<CreateHolidayDTO, HolidayEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}