using Absence.Application.UseCases.AbsenceTypes.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.Mappings;

internal class AbsenceTypeMapping : Profile
{
    public AbsenceTypeMapping()
    {
        CreateMap<AbsenceTypeEntity, AbsenceTypeDTO>();
    }
}