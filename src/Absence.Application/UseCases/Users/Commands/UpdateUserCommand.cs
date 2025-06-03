using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public class UpdateUserCommand(UserDetails userDetails) : IRequest
{
    public UserDetails UserDetails { get; } = userDetails;
}