using Absence.Application.UseCases.Absences.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Absences.Mappings;

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

        CreateMap<CreateAbsenceDTO, AbsenceRequestEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceId, opt => opt.Ignore())
            .ForMember(dest => dest.RequestType, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.NewAbsenceTypeId, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.Organization))
            .ForMember(dest => dest.Organization, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<EditAbsenceDTO, AbsenceRequestEntity>()
            .ForMember(dest => dest.AbsenceId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.NewAbsenceTypeId, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
            .ForMember(dest => dest.Organization, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.RequestType, opt => opt.Ignore());

        CreateMap<EditAbsenceDTO, AbsenceEntity>()
            .ForMember(dest => dest.AbsenceTypeId, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceType, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Organization, opt => opt.Ignore());

        CreateMap<AbsenceRequestEntity, AbsenceDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AbsenceId ?? 0));

        CreateMap<AbsenceEntity, AbsenceDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.AbsenceTypeId));

        CreateMap<AbsenceEntity, AbsenceRequestEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RequestType, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceId, opt => opt.MapFrom(src => src.Id));

        CreateMap<AbsenceRequestEntity, AbsenceEventDTO>()
            .ForMember(dest => dest.AbsenceType, opt => opt.MapFrom(src => src.NewAbsenceTypeId))
            .ForMember(dest => dest.AbsenceEventType, opt => opt.MapFrom(src => (int)src.RequestType))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Email));

        CreateMap<AbsenceRequestEntity, AbsenceEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceType, opt => opt.Ignore());
    }
}