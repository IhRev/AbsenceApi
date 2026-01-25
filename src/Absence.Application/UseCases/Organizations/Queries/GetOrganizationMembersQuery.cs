using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Queries;

public record GetOrganizationMembersQuery(int OrganizationId) : IRequest<OneOf<Success<IEnumerable<MemberDTO>>, BadRequest>>;