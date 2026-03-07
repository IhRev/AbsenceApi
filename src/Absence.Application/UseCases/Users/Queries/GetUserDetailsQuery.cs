using Absence.Application.Common.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Queries;

public class GetUserDetailsQuery : IRequest<OneOf<UserDetails, NotFound>>;