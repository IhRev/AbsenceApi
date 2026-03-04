using Absence.Application.UseCases.Permissions.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Permissions.Mappings;

internal class PermissionMapping : Profile
{
    public PermissionMapping()
    {
        CreateMap<PermissionEntity, PermissionDTO>();
    }
}