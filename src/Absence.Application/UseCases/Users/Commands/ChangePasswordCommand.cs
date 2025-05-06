using Absence.Application.Common.Results;
using Absence.Application.UseCases.Users.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Commands;

public class ChangePasswordCommand(ChangePasswordRequest request) : IRequest<OneOf<Success, NotFound, BadRequest>>
{
    public ChangePasswordRequest Request { get; } = request;
} 