using Absence.Application.Common.Results;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Queries;

public record GetAllAbsenceTypesQuery(int OrganizationId) : IRequest<OneOf<Success<IEnumerable<AbsenceTypeDTO>>, BadRequest>>;