using Absence.Application.UseCases.Holidays.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Holidays.Mappings;

internal class HolidayMapping : Profile
{
    public HolidayMapping()
    {
        CreateMap<HolidayEntity, HolidayDTO>();

        CreateMap<EditHolidayDTO, HolidayEntity>()
            .ForMember(dest => dest.OrganizationId, opt => opt.Ignore());

        CreateMap<CreateHolidayDTO, HolidayEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}