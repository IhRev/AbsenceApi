using Absence.Infrastructure.Entities;
using AutoMapper;

namespace Absence.Api.Features.Absences;

internal class AbsenceMapping : Profile
{
    public AbsenceMapping()
    {
        CreateMap<CreateAbsenceDTO, AbsenceEntity>()
            .ForMember(dest => dest.AbsenceTypeId, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.Organization))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceType, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Organization, opt => opt.Ignore());

        CreateMap<CreateAbsenceDTO, AbsenceEventEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceId, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceEventType, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceTypeId, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.Organization))
            .ForMember(dest => dest.Organization, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<EditAbsenceDTO, AbsenceEventEntity>()
            .ForMember(dest => dest.AbsenceId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceTypeId, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.AbsenceEventType, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
            .ForMember(dest => dest.Organization, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<EditAbsenceDTO, AbsenceEntity>()
            .ForMember(dest => dest.AbsenceTypeId, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceType, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Organization, opt => opt.Ignore());

        CreateMap<AbsenceEventEntity, AbsenceDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AbsenceId ?? 0));

        CreateMap<AbsenceEntity, AbsenceDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.AbsenceTypeId));

        CreateMap<AbsenceEntity, AbsenceEventEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceEventType, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceId, opt => opt.MapFrom(src => src.Id));

        CreateMap<AbsenceEventEntity, AbsenceEventDTO>()
            .ForMember(dest => dest.AbsenceType, opt => opt.MapFrom(src => src.AbsenceTypeId))
            .ForMember(dest => dest.AbsenceEventType, opt => opt.MapFrom(src => src.AbsenceEventType))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Email));

        CreateMap<AbsenceEventEntity, AbsenceEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceType, opt => opt.Ignore());
    }
}
