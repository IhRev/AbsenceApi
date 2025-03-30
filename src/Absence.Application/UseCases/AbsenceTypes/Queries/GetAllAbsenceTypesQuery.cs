using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.AbsenceTypes.Queries;

public class GetAllAbsenceTypesQuery : IRequest<IEnumerable<AbsenceTypeDTO>>;