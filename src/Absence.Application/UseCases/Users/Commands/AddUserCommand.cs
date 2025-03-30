using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public class AddUserCommand(CreateUserDTO user) : IRequest<string>
{
    public CreateUserDTO User { get; } = user;
}