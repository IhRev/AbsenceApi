using Absence.Application.UseCases.Holidays.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Holidays.Mappings;

internal class HolidayMapping : Profile
{
    public HolidayMapping()
    {
        CreateMap<EventEntity, HolidayDTO>();

        CreateMap<EditEventDTO, EventEntity>()
            .ForMember(dest => dest.OrganizationId, opt => opt.Ignore());

        CreateMap<CreateHolidayDTO, EventEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}