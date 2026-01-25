using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Holidays.Commands;

public record EditHolidayCommand(EditHolidayDTO Holiday) : IRequest<OneOf<Success, NotFound, AccessDenied>>;