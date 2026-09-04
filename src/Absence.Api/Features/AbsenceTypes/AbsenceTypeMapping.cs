using Absence.Infrastructure.Entities;
using AutoMapper;

namespace Absence.Api.Features.AbsenceTypes;

internal class AbsenceTypeMapping : Profile
{
    public AbsenceTypeMapping()
    {
        CreateMap<AbsenceTypeEntity, AbsenceTypeDTO>();
    }
}
