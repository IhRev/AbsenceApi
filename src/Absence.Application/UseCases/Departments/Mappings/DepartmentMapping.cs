using Absence.Application.UseCases.Departments.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Departments.Mappings;

internal class DepartmentMapping : Profile
{
    public DepartmentMapping()
    {
        CreateMap<DepartmentEntity, DepartmentDTO>();
        CreateMap<EditDepartmentDTO, DepartmentEntity>();
        CreateMap<CreateDepartmentDTO, DepartmentEntity>();
    }
}