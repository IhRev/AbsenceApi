using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Queries;

public class GetUserDetailsQuery : IRequest<UserDetails>;