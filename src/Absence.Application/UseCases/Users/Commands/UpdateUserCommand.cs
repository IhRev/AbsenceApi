using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public record UpdateUserCommand(UserDetails UserDetails) : IRequest;