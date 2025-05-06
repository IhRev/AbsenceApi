using Absence.Application.Common.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.Mappings;

internal class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<UserEntity, UserDetails>();
    }
}