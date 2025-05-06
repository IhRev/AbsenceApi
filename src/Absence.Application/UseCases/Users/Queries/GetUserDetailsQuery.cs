using Absence.Application.Common.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Users.Queries;

public class GetUserDetailsQuery : IRequest<OneOf<Success<UserDetails>, NotFound>>;