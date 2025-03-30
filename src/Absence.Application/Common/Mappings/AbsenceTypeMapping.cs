using Absence.Application.Common.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.Common.Mappings;

internal class AbsenceTypeMapping : Profile
{
    public AbsenceTypeMapping()
    {
        CreateMap<AbsenceTypeEntity, AbsenceTypeDTO>();
    }
}