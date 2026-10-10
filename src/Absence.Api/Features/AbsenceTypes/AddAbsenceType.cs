using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using Absence.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.AbsenceTypes;

public class CreateAbsenceTypeRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(30)]
    public required string Name { get; set; }
}

public static class AddAbsenceType
{
    public sealed class Command(int organizationId, CreateAbsenceTypeRequest request)
        : IRequest<OneOf<Success<int>, NotFound, BadRequest, AccessDenied>>
    {
        public int OrganizationId { get; } = organizationId;
        public CreateAbsenceTypeRequest Request { get; } = request;
    }

    internal sealed class Handler(AbsenceContext db, IOrganizationAccess organizationAccess)
        : IRequestHandler<Command, OneOf<Success<int>, NotFound, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success<int>, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireAdminAsync(request.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out var denied))
            {
                return denied.Match<OneOf<Success<int>, NotFound, BadRequest, AccessDenied>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
            }

            var nameTaken = await db.AbsenceTypes.AnyAsync(
                _ => _.OrganizationId == request.OrganizationId && _.Name == request.Request.Name,
                cancellationToken);
            if (nameTaken)
            {
                return new BadRequest($"An absence type named '{request.Request.Name}' already exists.");
            }

            var type = new AbsenceTypeEntity
            {
                Name = request.Request.Name,
                OrganizationId = request.OrganizationId
            };
            db.AbsenceTypes.Add(type);
            await db.SaveChangesAsync(cancellationToken);

            return new Success<int>(type.Id);
        }
    }
}
