using Absence.Infrastructure.Entities;
using AutoMapper;

namespace Absence.Api.Features.Holidays;

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
