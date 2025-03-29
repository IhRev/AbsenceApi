using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.AbsenceTypes.Queries;

internal class GetAllAbsenceTypesQuery : IRequest<IEnumerable<AbsenceTypeDTO>>;