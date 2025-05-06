using Absence.Application.Common.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Commands;

public class UpdateUserCommand(UserDetails userDetails) : IRequest<OneOf<Success, NotFound>>
{
    public UserDetails UserDetails { get; } = userDetails;
}