using Absence.Application.UseCases.Users.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Commands;

public record AddUserCommand(RegisterDTO User) : IRequest<OneOf<Success, Error<string>>>;