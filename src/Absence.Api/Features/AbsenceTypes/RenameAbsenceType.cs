using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.AbsenceTypes;

public class RenameAbsenceTypeRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(30)]
    public required string Name { get; set; }
}

public static class RenameAbsenceType
{
    public sealed class Command(int organizationId, int typeId, RenameAbsenceTypeRequest request)
        : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public int OrganizationId { get; } = organizationId;
        public int TypeId { get; } = typeId;
        public RenameAbsenceTypeRequest Request { get; } = request;
    }

    internal sealed class Handler(AbsenceContext db, IOrganizationAccess organizationAccess)
        : IRequestHandler<Command, OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireAdminAsync(request.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out var denied))
            {
                return denied.Match<OneOf<Success, NotFound, BadRequest, AccessDenied>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
            }

            var type = await db.AbsenceTypes.FirstOrDefaultAsync(
                _ => _.Id == request.TypeId && _.OrganizationId == request.OrganizationId,
                cancellationToken);
            if (type is null)
            {
                return new NotFound();
            }

            if (await db.AbsenceTypes.AnyAsync(
                _ => _.OrganizationId == request.OrganizationId && _.Name == request.Request.Name && _.Id != type.Id,
                cancellationToken))
            {
                return new BadRequest($"An absence type named '{request.Request.Name}' already exists.");
            }

            type.Name = request.Request.Name;
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
