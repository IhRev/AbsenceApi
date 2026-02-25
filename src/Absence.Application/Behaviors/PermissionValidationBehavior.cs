using Absence.Application.Common.Interfaces;
using Absence.Application.Exceptions;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using MediatR;

namespace Absence.Application.Behaviors;

internal class PermissionValidationBehavior<TRequest, TResponse>(
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IUser user
) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IRequirePermission permissionRequest)
        {
            var hasPermission = await userOrganizationRoleRepository.HasPermission(
                permissionRequest.OrganizationId, 
                user.ShortId, 
                permissionRequest.Permission, 
                cancellationToken
            );

            if (!hasPermission)
            {
                throw new AccessDeniedException();
            }
        }

        return await next();
    }
}