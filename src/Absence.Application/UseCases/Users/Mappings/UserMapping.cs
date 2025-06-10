﻿using Absence.Application.Common.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Users.Mappings;

internal class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<UserEntity, UserDetails>();
    }
}