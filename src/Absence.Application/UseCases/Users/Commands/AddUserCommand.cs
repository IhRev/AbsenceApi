using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public class AddUserCommand(RegisterDTO user) : IRequest<string>
{
    public RegisterDTO User { get; } = user;
}