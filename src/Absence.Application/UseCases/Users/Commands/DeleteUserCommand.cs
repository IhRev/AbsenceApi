using Absence.Application.Common.Results;
using Absence.Application.UseCases.Users.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Commands;

public record DeleteUserCommand(DeleteUserRequest Request) : IRequest<OneOf<Success, BadRequest>>;