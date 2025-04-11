using Absence.Application.Common.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Commands;

public class AddUserCommand(RegisterDTO user) : IRequest<OneOf<Success, Error<string>>>
{
    public RegisterDTO User { get; } = user;
}