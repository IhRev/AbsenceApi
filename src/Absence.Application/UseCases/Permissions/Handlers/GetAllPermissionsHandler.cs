using Absence.Application.UseCases.Permissions.DTOs;
using Absence.Application.UseCases.Permissions.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Permissions.Handlers;

internal class GetAllPermissionsHandler(IRepository<PermissionEntity> permissionRepository, IMapper mapper) 
    : IRequestHandler<GetAllPermissionsQuery, IEnumerable<PermissionDTO>>
{
    public async Task<IEnumerable<PermissionDTO>> Handle(
        GetAllPermissionsQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await permissionRepository.GetAsync(cancellationToken);
        return mapper.Map<IEnumerable<PermissionDTO>>(permissions);
    }
}