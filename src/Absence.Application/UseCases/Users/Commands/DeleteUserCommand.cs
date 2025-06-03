using Absence.Application.Common.Results;
using Absence.Application.UseCases.Users.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Commands;

public class DeleteUserCommand(DeleteUserRequest request) : IRequest<OneOf<Success, BadRequest>>
{
    public DeleteUserRequest Request { get; } = request;
}