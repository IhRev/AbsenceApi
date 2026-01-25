using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Organizations.Queries;

public record GetUserOrganizationsQuery : IRequest<IEnumerable<OrganizationDTO>>;